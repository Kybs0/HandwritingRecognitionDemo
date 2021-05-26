using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Ink;
using System.Windows.Threading;

namespace HandwritingRecognitionDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void HandwritingInkCanvas_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Analyze();
        }
        private void Analyze()
        {
            if (HandwritingInkCanvas.Strokes.Count == 0)
            {
                return;
            }
            var stroke = GetCombinedStore(HandwritingInkCanvas.Strokes);
            var results = Recognize(stroke);
            var resultList = results.Distinct().ToList();
            RecognizedTextBlock.Text = string.Join(" ", resultList);
        }

        private string[] Recognize(StrokeCollection strokes)
        {
            using (InkAnalyzer theInkAnalyzer = new InkAnalyzer())
            {
                theInkAnalyzer.AddStrokes(strokes, 0x0804); //0x0804表示简体中文
                theInkAnalyzer.SetStrokesType(strokes, StrokeType.Writing);
                AnalysisStatus status = theInkAnalyzer.Analyze();
                if (status.Successful)
                {
                    return theInkAnalyzer.GetAlternates().OfType<AnalysisAlternate>().Select(X => X.RecognizedString).ToArray();
                }
                return new string[0];
            }
        }

        internal Stroke GetCombinedStore(StrokeCollection strokes)
        {
            if (strokes == null || strokes.Count == 0)
            {
                return null;
            }

            var points = new StylusPointCollection();
            foreach (var stroke in strokes)
            {
                points.Add(stroke.StylusPoints);
            }

            return new Stroke(points);
        }
        internal string[] Recognize(Stroke stroke)
        {
            if (stroke == null)
            {
                return null;
            }

            using (InkAnalyzer theInkAnalyzer = new InkAnalyzer())
            {
                theInkAnalyzer.AddStroke(stroke, 0x0804); //0x0804表示简体中文
                theInkAnalyzer.SetStrokeType(stroke, StrokeType.Writing);
                AnalysisStatus status = theInkAnalyzer.Analyze();
                if (status.Successful)
                {
                    return theInkAnalyzer.GetAlternates().OfType<AnalysisAlternate>().Select(X => X.RecognizedString).ToArray();
                }
                return new string[0];
            }
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            HandwritingInkCanvas.Strokes.Clear();
            RecognizedTextBlock.Text = string.Empty;
        }

    }
}
