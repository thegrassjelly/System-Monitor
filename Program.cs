using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Resources;
using System.Speech.Synthesis;

namespace Monitor
{
    class Program
    {
        static SpeechSynthesizer synth = new SpeechSynthesizer();
        static void Main(string[] args)
        {
            #region Misc vars
            // rand generator init
            var rand = new Random();
            // Speech speed var for incrementing
            var speechSpeed = 1;
            #endregion

            synth.SelectVoiceByHints(VoiceGender.Female);
            synth.Speak("Hello");

            #region Max cpu message list
            // List for max cpu messages
            var cpuMaxMsgs = new List<string>();
            cpuMaxMsgs.Add("WARNING: Holy crap cour CPU is about to catch fire!");
            cpuMaxMsgs.Add("WARNING: Oh my god you should not run your CPU that hard");
            cpuMaxMsgs.Add("WARNING: Stop downloading the porn it's maxing me out");
            cpuMaxMsgs.Add("WARNING: Your CPU is chasing squirrels");
            cpuMaxMsgs.Add("RED ALERT! RED ALERT! RED ALERT! I FARTED");
            #endregion

            #region Counter vars
            // Pull current CPU load in percentage
            var perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCpuCount.NextValue();
            // Pull current avail memory in MB
            var perfMemCount = new PerformanceCounter("Memory", "Available Mbytes");
            perfMemCount.NextValue();
            // Pull system uptime (in seconds)
            var perfUptimeCount = new PerformanceCounter("System", "System Up Time");
            perfUptimeCount.NextValue();
            #endregion

            #region Uptime formatting
            // Uptime counter variables
            // Timespan to easily format seconds in catergories as seen in the string.format
            var uptimeSpan = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
            var systemUptimeMsg = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds", 
                    (int)uptimeSpan.TotalDays, 
                    (int)uptimeSpan.Hours, 
                    (int)uptimeSpan.Minutes, 
                    (int)uptimeSpan.Seconds
                    );
            synth.Speak(systemUptimeMsg);
            #endregion

            #region Main loop
            // Infinite While Loop
            while (true)
            {
                // Counters variables for unique access
                var currentCpuPercentage = (int)perfCpuCount.NextValue();
                var currentAvailMem = (int)perfMemCount.NextValue();

                // Voice variable inits
                var cpuLoadVocalMsg = string.Format("The current CPU load is {0} percent", currentCpuPercentage);
                var memAvailVoiceMsg = string.Format("You currently have {0} megabytes of memory available", currentCpuPercentage);

                // Console displays
                Console.WriteLine("CPU Load        : {0}%", currentCpuPercentage);
                Console.WriteLine("Available Memory: {0}MB", currentAvailMem.ToString("##,##"));
                Console.WriteLine("");

                // If CPU load is above 80%
                if (currentCpuPercentage > 80)
                {
                    // If maxed out CPU (100% load)
                    if (currentCpuPercentage == 100)
                    {
                        // To prevent  speech speed from exceeding 5x normal
                        if (speechSpeed < 5)
                        {
                            // Increment + 1 to itself
                            speechSpeed++;
                        }
                        // Speak overload with custom speech speed (rate)
                        Speak(cpuMaxMsgs[rand.Next(4)], VoiceGender.Male, speechSpeed++);
                        OpenWebsite("http://www.google.com");
                    }
                    // Default speak for above 80% CPU load
                    else
                    {
                        // Speak overload using default speech speed (rate)
                        Speak(cpuLoadVocalMsg, VoiceGender.Male);
                    }
                }
                // If current available memory is less than 1024MB
                if (currentAvailMem < 1024)
                {
                    Speak(memAvailVoiceMsg, VoiceGender.Male, 3);
                }

                // Display/Run pause of 1 second (1000ms)
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Functions
        // Speak function w/ default voice (female) and default rate
        public static void Speak(string msg)
        {
            synth.Speak(msg);
        }

        // Speak function w/ voice gender and rate overload
        public static void Speak(string msg, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            Speak(msg, voiceGender);
        }

        // Speak function w/ voice gender and default rate
        public static void Speak(string msg, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(msg);
        }

        // Open browser (chrome) and enter website function
        public static void OpenWebsite(string url)
        {
            var p1 = new Process();
            p1.StartInfo.FileName = "chrome.exe";
            p1.StartInfo.Arguments = url;
            p1.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p1.Start();
        }
        #endregion
    }
}
