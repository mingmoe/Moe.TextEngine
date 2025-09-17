using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Icu;

namespace Moe.TextEngine
{
    public sealed class LayoutEngine
    {
        public RenderEngine RenderEngine { get; init; }

        public LibIcu Icu { get; init; }

        public LayoutEngine(RenderEngine renderEngine)
        {
            ArgumentNullException.ThrowIfNull(renderEngine);
            RenderEngine = renderEngine;
            Icu = new LibIcu();
        }

        private int SplitLine(BreakIterator it,ShapeRun run, int width)
        {
            var used = run.UsedText;
            it.SetText(used); 
            it.MoveFirst();

            int latestPosition = it.Current;

            while (true)
            {
                if (latestPosition == BreakIterator.DONE)
                {
                    return used.Length;
                }

                var nextRenderWidth = RenderEngine.MeasureSize(run.ResetUsedText(run.Offset, 
                    Helper.ConvertCharacterLengthToUtf16Length(used, 0,latestPosition))).X;

                if (nextRenderWidth > width)
                {
                    break;
                }

                latestPosition = it.MoveNext();
            }

            return Helper.ConvertCharacterLengthToUtf16Length(used,0,latestPosition);
        }

        public List<ShapeRun> SplitLines(ShapeRun run,int width)
        {
            ShapeOptions options = run.ShapeOptions ?? new ShapeOptions();

            Locale locale = new(options.Language, options.Region);

            BreakIterator it = BreakIterator.CreateLineInstance(locale);

            List<ShapeRun> lines = [];

            var consumed = 0;
            var total = run.UsedText.Length;

            while (consumed != total)
            {
                var consume = SplitLine(it, run.ResetUsedText(run.Offset+consumed, total - consumed), width);

                lines.Add(new (run.FontOptions,run.ShapeOptions)
                {
                    Text = run.Text,
                    Offset = run.Offset + consumed,
                    Length = consume,
                });

                consumed += consume;
            }

            return lines;
        }

    }
}
