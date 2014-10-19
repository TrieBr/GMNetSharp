using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Threading;
namespace GMNetSharp
{
    public  static class Logger
    {
        //Writer (Console, File, Form, etc) this logger is writing to
        private static TextWriter writer = Console.Out;
        //Mutex Lock (Since logs can be written from other threads)
        private static Mutex mutex = new Mutex();
        //Existing TextWriter
        public static void SetWriter(TextWriter writerObject)
        {
            writer = writerObject;
        }
        //Write a trace line
        public static void Trace(string value)
        {
           mutex.WaitOne();
           writer.WriteLine("[TRACE] " +value);
           mutex.ReleaseMutex();  
        }
        //Write a warn line
        public static void Warn(string value)
        {
            mutex.WaitOne();
            writer.WriteLine("[WARN] "+value);
            mutex.ReleaseMutex();
        }
    }
}
