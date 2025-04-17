using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TestGG.Utilities
{
    public class ErrorLineBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _editor;
        private int _highlightedLine = -1;
        private string _issueType;

        public ErrorLineBackgroundRenderer(TextEditor editor)
        {
            _editor = editor;
        }

        public void SetHighlightedLine(int lineNumber, string issueType = null)
        {
            if (lineNumber < 1 || lineNumber > _editor.Document.LineCount)
            {
                Debug.WriteLine($"Invalid line number: {lineNumber}");
                return;
            }

            _highlightedLine = lineNumber;
            _issueType = issueType;
            _editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_highlightedLine == -1 || textView == null || textView.VisualLines == null)
                return;

            var documentLine = _editor.Document.GetLineByNumber(_highlightedLine);
            if (documentLine == null)
                return;

            var backgroundGeometry = BackgroundGeometryBuilder.GetRectsForSegment(textView, new TextSegment
            {
                StartOffset = documentLine.Offset,
                EndOffset = documentLine.EndOffset
            });

            var brush = GetHighlightBrush(_issueType);
            brush.Opacity = 0.5; // Make it semi-transparent

            foreach (var rect in backgroundGeometry)
            {
                drawingContext.DrawRectangle(brush, null, rect);
            }
        }

        private SolidColorBrush GetHighlightBrush(string issueType)
        {
            var colorMap = new Dictionary<string, Color>
        {
            { "Syntax Error", Colors.Red },
            { "Logic Error", Colors.Orange },
            { "Join Error", Colors.Blue },
            { "Missing Nolock", Colors.Cyan },
            { "Temp Table Not Dropped", Colors.Magenta },
            { "Select Star Usage", Colors.Yellow },
            { "Index Suggestion", Colors.Green },
            { "Expensive Sort Operation", Colors.Pink },
            { "Security Issue", Colors.Gold },
            {"magic" ,Colors.Purple }
        };

            return new SolidColorBrush(colorMap.ContainsKey(issueType) ? colorMap[issueType] : Colors.Gray);
        }

        public void ClearHighlights()
        {
            _highlightedLine = -1; 
            _issueType = null;    
            _editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }
    }
}
