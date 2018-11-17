using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kazedan.Construct;
using SlimDX.Direct2D;

namespace Kazedan.Graphics.Renderer
{
    public abstract class NoteRenderer
    {
        public abstract void Render(RenderTarget target, List<Note> notes, MIDIKeyboard keyboard);
    }
}
