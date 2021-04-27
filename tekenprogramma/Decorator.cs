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

        public void Draw(FrameworkElement element, string ornament, string position, Invoker invoker)
        {
            TextBlock lab = new TextBlock();
            lab.Text = ornament;

            string gs = lab.Text.Substring(0,5);

            //if ornament add to goup
            if (gs =="groep")
            {
                Group lastgroup = invoker.selectedGroups.Last();

                double groupleft = 1000;
                double grouptop = 1000;
                double groupwidth = 0;
                double groupheight = 0;
                double groupright = 0;
                double groupbottom = 0;
                //calculate group size
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    if (Convert.ToDouble(elm.ActualOffset.X) < groupleft)
                    {
                        groupleft = Convert.ToDouble(elm.ActualOffset.X);
                    }
                    if (Convert.ToDouble(elm.ActualOffset.Y) < grouptop)
                    {
                        grouptop = Convert.ToDouble(elm.ActualOffset.Y);
                    }
                    if (Convert.ToDouble(elm.ActualOffset.X) > groupright)
                    {
                        groupright = Convert.ToDouble(elm.ActualOffset.X);
                        groupwidth = (groupright - groupleft) + Convert.ToDouble(elm.Width);
                    }
                    if (Convert.ToDouble(elm.ActualOffset.Y) > groupbottom)
                    {
                        groupbottom = Convert.ToDouble(elm.ActualOffset.Y);
                        groupheight = (groupbottom - grouptop) + Convert.ToDouble(elm.Height);
                    }
                }
                //set position
                if (position == "top")
                {
                    Canvas.SetLeft(lab, groupright - groupleft);
                    Canvas.SetTop(lab, grouptop - 25);
                }
                else if (position == "bottom")
                {
                    Canvas.SetLeft(lab, groupright - groupleft);
                    Canvas.SetTop(lab, grouptop + groupheight + 25);
                }
                else if (position == "left")
                {
                    Canvas.SetLeft(lab, groupleft - 25);
                    Canvas.SetTop(lab, groupbottom - grouptop);
                }
                else if (position == "right")
                {
                    Canvas.SetLeft(lab, groupleft + groupwidth + 25);
                    Canvas.SetTop(lab, groupbottom - grouptop);
                }
                //add to canvas
                Canvas parent = (Canvas)element.Parent;
                parent.Children.Add(lab);

            }
            //else ornament add to element
            else
            {
                if (position == "top")
                {
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
                //add to canvas
                Canvas parent = (Canvas)element.Parent;
                parent.Children.Add(lab);
            }



        }

        public void setOrnament()
        {

        }
    }

}
