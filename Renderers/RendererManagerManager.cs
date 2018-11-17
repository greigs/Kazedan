using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Kazedan.Graphics;
using Kazedan.Graphics.Renderer;
using Sanford.Multimedia.Midi;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;

namespace Kazedan.Construct
{
    public class RendererManagerManager : IRendererManager
    {
        private readonly NoteRenderer[] _noteRenderers;
        private readonly NoteManager _noteManager;
        private readonly MIDIKeyboard _keyboard;
        private readonly KeyRenderer[] _keyRenderers;
        private readonly Sequence _sequence;
        private readonly Sequencer _sequencer;


        public RendererManagerManager( NoteManager noteManager, MIDIKeyboard keyboard, Sequence sequence, Sequencer sequencer)
        {
            _noteManager = noteManager;
            _keyboard = keyboard;
            _sequence = sequence;
            _sequencer = sequencer;
            _noteRenderers = new NoteRenderer[] {new FastNoteRenderer(), new FancyNoteRenderer()};
            _keyRenderers = new KeyRenderer[] {new FastKeyRenderer(), new FancyKeyRenderer()};
        }

        public void Render(RenderTarget target)
        {
            // Fill background depending on render mode
            if (_noteManager.RenderFancy)
                target.FillRectangle(GFXResources.BackgroundGradient, new RectangleF(PointF.Empty, target.Size));
            else
                target.Clear(Color.Black);

            // Render notes and keyboard display
            lock (_noteManager.Notes)
            {
                if (_noteManager.RenderFancy)
                    _noteRenderers[1].Render(target, _noteManager.Notes, _keyboard);
                else
                    _noteRenderers[0].Render(target, _noteManager.Notes, _keyboard);
            }
            lock (_keyboard.KeyPressed)
            {
                if (_noteManager.RenderFancy)
                    _keyRenderers[1].Render(target, _keyboard.KeyPressed);
                else
                    _keyRenderers[0].Render(target, _keyboard.KeyPressed);
            }

            // Draw time progress bar
            if (_sequence?.GetLength() > 0)
            {
                float percentComplete = 1f * _sequencer.Position / _sequence.GetLength();
                target.FillRectangle(GFXResources.DefaultBrushes[5],
                    new RectangleF(GFXResources.ProgressBarBounds.X, GFXResources.ProgressBarBounds.Y, GFXResources.ProgressBarBounds.Width * percentComplete, GFXResources.ProgressBarBounds.Height));
                target.DrawRectangle(GFXResources.DefaultBrushes[2], GFXResources.ProgressBarBounds, .8f);
            }

            // Render debug information
            string[] debug;
            string usage = Application.ProductName + " " + Application.ProductVersion + " (c) " + Application.CompanyName;
            //if (ShowDebug)
            //{
            //    debug = new[]
            //    {
            //        usage,
            //        "      file: " + MIDIFile,
            //        "note_count: " + NoteManager.Notes.Count,
            //        "  frames/s: " + (Kazedan.Elapsed == 0 ? "NaN" : 1000 / Kazedan.Elapsed + "") +" fps",
            //        "  renderer: " + (NoteManager.RenderFancy ? "fancy" : NoteManager.UserEnabledFancy ? "forced-fast" : "fast"),
            //        "  seq_tick: " + (sequence == null ? "? / ?" : $"{sequencer.Position} / {sequence.GetLength()}"),
            //        "     delay: " + Delay + "ms",
            //        "       kbd: " + GFXResources.NoteCount + " key(s) +" + GFXResources.NoteOffset + " offset",
            //        "   stopped: " + Stopped
            //    };

            //}
            //else
            {
                debug = new[] { usage };
            }
            string debugText = debug.Aggregate("", (current, ss) => current + ss + '\n');
            target.DrawText(debugText, GFXResources.DebugFormat, GFXResources.DebugRectangle, GFXResources.DefaultBrushes[0], DrawTextOptions.None,
                MeasuringMethod.Natural);

            // Render large title text
            //if (LoadingStatus == 0)
            //    target.DrawText("INITIALIZING MIDI DEVICES", GFXResources.HugeFormat, GFXResources.FullRectangle, GFXResources.DefaultBrushes[0], DrawTextOptions.None, MeasuringMethod.Natural);
            //else if (LoadingStatus > 0 && LoadingStatus < 100)
            //    target.DrawText("LOADING " + LoadingStatus + "%", GFXResources.HugeFormat, GFXResources.FullRectangle, GFXResources.DefaultBrushes[0], DrawTextOptions.None, MeasuringMethod.Natural);
        }
    }
}