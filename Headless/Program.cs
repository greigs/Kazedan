using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kazedan.Construct;

namespace Headless
{
    class Program
    {
        private static MIDISequencer sequencer;
        static void Main(string[] args)
        {
            
            sequencer = new MIDISequencer();
            sequencer.LoadCompleted += Sequencer_LoadCompleted;
            Thread t = new Thread(() => { sequencer.Init(); });
            t.Start();
            t.Join();

            


            sequencer.ShowDebug = true;
            Debug.WriteLine("initialised midi");
            bool exists = File.Exists("C:/Users/greig/Documents/Piano/Assets/bach.mid");
            sequencer.Load("C:\\Users\\greig\\Documents\\Piano\\Assets\\bach.mid");
            var file = sequencer.MIDIFile;
            //sequencer.Jump(0);
            

            Console.ReadLine();

        }

        private static void Sequencer_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            sequencer.Start();
            Thread t = new Thread(() =>
            {
                while (!sequencer.Stopped)
                    // Do sequencer tick
                {
                    sequencer.UpdateNotePositions();
                    sequencer.UpdateRenderer();
                }
                Thread.Sleep(10);
            });
            t.Start();
            t.Join();
        }
    }
}
