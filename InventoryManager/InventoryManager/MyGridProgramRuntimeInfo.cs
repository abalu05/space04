#line hidden
using System;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers00
{
    /// <summary>
    /// Provides runtime info for a running grid program.
    /// </summary>
    public class MyGridProgramRuntimeInfo
    {
        /// <summary>
        /// Gets the time elapsed since the last time the Main method of this program was run. This property returns no
        /// valid data neither in the constructor nor the Save method.
        /// </summary>
        TimeSpan TimeSinceLastRun { get { return new TimeSpan(0); } }

        /// <summary>
        /// Gets the number of milliseconds it took to execute the Main method the last time it was run. This property returns no valid
        /// data neither in the constructor nor the Save method.
        /// </summary>
        double LastRunTimeMs { get { return 0.0; } }

        /// <summary>
        /// Gets the maximum number of significant instructions that can be executing during a single run, including
        /// any other programmable blocks invoked immediately.
        /// </summary>
        int MaxInstructionCount { get { return 0; } }

        /// <summary>
        /// Gets the current number of significant instructions executed.
        /// </summary>
        int CurrentInstructionCount { get { return 0; } }

        /// <summary>
        /// Gets the maximum number of method calls that can be executed during a single run, including
        /// any other programmable blocks invoked immediately.
        /// </summary>
        int MaxMethodCallCount { get { return 0; } }

        /// <summary>
        /// Gets the current number of method calls.
        /// </summary>
        int CurrentMethodCallCount { get { return 0; } }

        private UpdateFrequency _UpdateFrequency = UpdateFrequency.None;
        public UpdateFrequency UpdateFrequency
        {
            get { return _UpdateFrequency; }
            set { _UpdateFrequency = value; }
        }
    }
}
#line default