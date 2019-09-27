using System.Windows;
using System.Windows.Controls;
using ElementWorldMaker.Existence.EnvironmentMaker;
using ElementWorldMaker.UIExtensions;

namespace ElementWorldMaker.EnvironmentViewing
{
    public class EtherVM : IEnvironmentVM, ICustomControls
    {
        public ElementPoint Size { get; protected set; }

        int[,,] _environment;

        int max = 0;

        public EtherVM(ElementPoint size, int[,,] environment)
        {
            Size = size;
            _environment = environment;

            timeTickButton.Click += OnTimeTickButton_Click;
            resetTimeButton.Click += OnResetTimeButton_Click;
        }

        public EnvironmentColor GetColor(int water, int wood, int wind)
        {
            int etherLevel = _environment[water, wood, wind];

            if (etherLevel > max)
            {
                max = etherLevel;
            }

            byte halfByte = byte.MaxValue / 2;

            byte etherByte = (byte)(halfByte * (8 - etherLevel) / 8.0);

            return new EnvironmentColor(etherByte, halfByte, halfByte);
        }

        readonly Button timeTickButton = new Button
        {
            Content = "Time Tick"
        };
        readonly Button resetTimeButton = new Button
        {
            Content = "Reset Time"
        };
        int timeTick = 0;
        readonly Label timeTickStringLiteral = new Label
        {
            Content = "Time Tick:"
        };
        readonly Label timeTickLabel = new Label
        {
            Content = "0"
        };

        public Control[] ExtraControls
        {
            get
            {
                return new Control[]
                {
                    timeTickButton,
                    resetTimeButton,
                    timeTickStringLiteral,
                    timeTickLabel
                };
            }
        }

        private void OnResetTimeButton_Click(object sender, RoutedEventArgs e)
        {
            timeTick = 0;
            timeTickLabel.Content = timeTick.ToString();
        }

        private void OnTimeTickButton_Click(object sender, RoutedEventArgs e)
        {
            timeTick++;
            timeTickLabel.Content = timeTick.ToString();
        }
    }
}