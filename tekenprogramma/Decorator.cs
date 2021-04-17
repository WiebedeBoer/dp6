using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace tekenprogramma
{
    public class Decorator
    {
        public Decorator()
        {

        }

        public void Draw()
        {

        }
    }

    public class OrnamentDecorator
    {
        public OrnamentDecorator()
        {

        }

        public void Draw(FrameworkElement element,string ornament, string position)
        {
            TextBlock lab = new TextBlock();
            lab.Text = ornament;

            if (position =="top")
            {

                //double left = element.ActualOffset.X;
                //double top = element.ActualOffset.Y;
                //double right = left + 50;
                //double bottom = top + 50;
                //lab.Margin = new Thickness(left, top, right, bottom);
                
                Canvas.SetLeft(lab, element.ActualOffset.X);
                Canvas.SetTop(lab, element.ActualOffset.Y - 25);
            }
            else if (position == "bottom")
            {

                Canvas.SetLeft(lab, element.ActualOffset.X);
                Canvas.SetTop(lab, element.ActualOffset.Y + element.Height + 25);
            }
            else if (position == "left")
            {

                Canvas.SetLeft(lab, element.ActualOffset.X - 25);
                Canvas.SetTop(lab, element.ActualOffset.Y);
            }
            else if (position == "right")
            {

                Canvas.SetLeft(lab, element.ActualOffset.X + element.Width + 25);
                Canvas.SetTop(lab, element.ActualOffset.Y);

            }
            Canvas parent = (Canvas)element.Parent;
            parent.Children.Add(lab);
        }

        public void setOrnament()
        {

        }
    }

}
