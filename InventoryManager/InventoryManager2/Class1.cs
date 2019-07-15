using System;
using System.Collections.Generic;

namespace InventoryManager2
{
    public class JSON
    {
        enum JSONPart { KEY, KEYEND, VALUE, VALUEEND }

        private int LastCharIndex;
        private IEnumerator<bool> Enumerator;
        public string Serialized { get; private set; }
        public JsonObject Result { get; private set; }

        public int Progress
        {
            get { return 100 * Math.Max(0, LastCharIndex) / Serialized.Length; }
        }

        public JSON(string serialized)
        {
            Serialized = serialized;
            Enumerator = Parse().GetEnumerator();
        }

        public bool ParsingComplete()
        {
            return !Enumerator.MoveNext();
        }

        public IEnumerable<bool> Parse()
        {

            LastCharIndex = -1;
            JSONPart Expected = JSONPart.VALUE;
            Stack<Dictionary<string, JsonObject>> ListStack = new Stack<Dictionary<string, JsonObject>>();
            Stack<JsonObject> JsonStack = new Stack<JsonObject>();
            JsonObject CurrentJsonObject = new JsonObject("");
            var trimChars = new char[] { '"', '\'', ' ', '\n', '\r', '\t' };
            var startTime = DateTime.Now;

            while (LastCharIndex < Serialized.Length - 1)
            {
                var charIndex = -1;
                switch (Expected)
                {
                    case JSONPart.VALUE:
                        charIndex = Serialized.IndexOfAny(new char[] { '{', '}', ',' }, LastCharIndex + 1);
                        if (charIndex == -1)
                            throw new UnexpectedCharacterException(new char[] { '{', '}', ',' }, "EOF", LastCharIndex);
                        //Console.WriteLine("Expecting Value...");
                        //Console.WriteLine("Found " + Serialized[charIndex] + " (" + charIndex + ")");
                        switch (Serialized[charIndex])
                        {
                            case '{':
                                CurrentJsonObject.SetValue(new Dictionary<string, JsonObject>());
                                JsonStack.Push(CurrentJsonObject);
                                CurrentJsonObject = new JsonObject();
                                Expected = JSONPart.KEY;
                                break;
                            case '}':
                            case ',':
                                var value = Serialized.Substring(LastCharIndex + 1, charIndex - LastCharIndex - 1).Trim(trimChars);
                                //Console.WriteLine("value is: '" + value + "'");
                                CurrentJsonObject.SetValue(value);

                                if (Serialized[charIndex] == '}')
                                {
                                    if (charIndex < Serialized.Length - 1 && Serialized[charIndex + 1] == ',')
                                        charIndex++;
                                    CurrentJsonObject = JsonStack.Pop();
                                }

                                Expected = JSONPart.KEY;
                                break;
                        }
                        LastCharIndex = charIndex;
                        break;
                    case JSONPart.KEY:
                        charIndex = Serialized.IndexOfAny(new char[] { '}', ':' }, LastCharIndex + 1);
                        //Console.WriteLine("Expecting Key...");
                        //Console.WriteLine("Found " + Serialized[charIndex] + " (" + charIndex + ")");
                        if (charIndex == -1)
                            throw new UnexpectedCharacterException(new char[] { '}', ':' }, "EOF", LastCharIndex);

                        switch (Serialized[charIndex])
                        {
                            case '}':
                                if (charIndex < Serialized.Length - 1 && Serialized[charIndex + 1] == ',')
                                    charIndex++;
                                CurrentJsonObject = JsonStack.Pop();
                                Expected = JSONPart.KEY;
                                break;
                            case ':':
                                var key = Serialized.Substring(LastCharIndex + 1, charIndex - LastCharIndex - 1).Trim(trimChars);
                                //Console.WriteLine("key is: '" + key + "'");
                                CurrentJsonObject = new JsonObject(key);
                                JsonStack.Peek().GetValue()
                                    .Add(CurrentJsonObject.Key, CurrentJsonObject);
                                Expected = JSONPart.VALUE;
                                break;
                        }
                        LastCharIndex = charIndex;
                        break;
                }
                //Console.WriteLine("Iteration done, CurrentJsonObject is: '" + CurrentJsonObject.Key + "'");
                if (DateTime.Now - startTime > TimeSpan.FromMilliseconds(30))
                {
                    yield return false;
                    startTime = DateTime.Now;
                }
            }

            Result = CurrentJsonObject;
            yield return true;
        }

        private class ParseException : Exception
        {
            public ParseException(string message, int position = -1)
                : base("PARSE ERROR" + (position == -1 ? "" : " after char " + position.ToString()) + ": " + message) { }

        }

        private class UnexpectedCharacterException : ParseException
        {
            public UnexpectedCharacterException(char[] expected, string received, int position = -1)
                : base("Expected one of [ '" + string.Join("', '", expected) + "' ] but received " + received + "!", position)
            { }

        }

    }

    public class JsonObject
    {
        public string Key { get; private set; }
        private string StringValue;
        private Dictionary<string, JsonObject> NestedValue;
        public bool IsFinal { get; private set; }

        public JsonObject(string key, string value)
        {
            Key = key;
            StringValue = value;
            IsFinal = true;
        }

        public JsonObject(string key, Dictionary<string, JsonObject> value)
        {
            Key = key;
            NestedValue = value;
            IsFinal = true;
        }

        public JsonObject(string key)
        {
            Key = key;
            IsFinal = false;
        }

        public JsonObject()
        {
            IsFinal = false;
        }

        public void SetKey(string key)
        {
            if (IsFinal)
                throw new Exception("JSON object can't be modified!");
            Key = key;
            if (StringValue != null || NestedValue != null)
                IsFinal = true;
        }

        public void SetValue(string value)
        {
            if (IsFinal)
                throw new Exception("JSON object can't be modified!");
            if (NestedValue != null)
                throw new Exception("Can't define JSON value and string value at the same time!");
            StringValue = value;
            IsFinal = (Key != null);
        }

        public void SetValue(Dictionary<string, JsonObject> value)
        {
            if (IsFinal)
                throw new Exception("JSON object can't be modified!");
            if (StringValue != null)
                throw new Exception("Can't define JSON value and string value at the same time!");
            NestedValue = value;
            IsFinal = (Key != null);
        }

        public string GetString()
        {
            return StringValue;
        }

        public Dictionary<string, JsonObject> GetValue()
        {
            return NestedValue;
        }

        override
        public string ToString()
        {
            return ToString(true);

        }

        public string ToString(bool pretty = true)
        {
            var result = "";
            if (Key != "" || StringValue != null) result = Key + (pretty ? ": " : ":");
            if (StringValue != null)
            {
                result += GetString();
            }
            else
            {
                result += "{";
                foreach (var kvp in GetValue())
                {
                    var childResult = kvp.Value.ToString(pretty);
                    if (pretty) childResult = childResult.Replace("\n", "\n  ");
                    result += (pretty ? "\n  " : "") + childResult + ",";
                }
                result = result.Substring(0, result.Length - 1);
                result += (pretty ? "\n}" : "}");
            }

            return result;
        }
    }
}