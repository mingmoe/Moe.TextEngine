using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine
{
    public sealed class LayoutEngine
    {
        public RenderEngine RenderEngine { get; init; }

        public LayoutEngine(RenderEngine renderEngine)
        {
            ArgumentNullException.ThrowIfNull(renderEngine);
            RenderEngine = renderEngine;
        }

        public List<ShapeRun> SplitLines(ShapeRun run)
        {
            List<ShapeRun> lines = [];

            return lines;
        }

    }
}
