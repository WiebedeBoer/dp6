using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace tekenprogramma
{

    public class Decorator : Baseshape
    {
        protected Baseshape component;

        //public Decorator(double x, double y, double width, double height) : base(x, y, width, height)
        //{

        //}

        public Decorator(double x, double y, double width, double height) : base(x, y, width, height)
        {

        }

        public override void Execute()
        {
            if (component != null)
            {
                component.Execute();
            }
        }
    }

    public class OrnamentDecorator : Decorator
    {

        //: base(x, y, width, height)
        //public OrnamentDecorator() 
        //{

        //}

        public OrnamentDecorator(double x, double y, double width, double height) : base(x, y, width, height)
        {

        }


        public void SetOrnament(FrameworkElement element, string ornament, string position, Invoker invoker)
        {
            if (ornament.Length > 5 && (position == "top" || position == "bottom" || position == "left" || position == "right" || position == "Top" || position == "Bottom" || position == "Left" || position == "Right"))
            {
                string gs = ornament.Substring(0, 5);
                //if ornament add to goup
                if (gs == "groep" || gs == "group")
                {
                    Group lastgroup = invoker.selectedGroups.Last();
                    lastgroup.Add(position, ornament);
                }
                //else ornament add to element
                else
                {
                    int inc = 0;
                    //int num = 0;
                    foreach (Strategy component in invoker.drawnComponents)
                    {
                        FrameworkElement fetched = component.GetElement();
                        if (fetched.AccessKey == element.AccessKey)
                        {
                            //num = inc;
                            component.Add(position, ornament);
                        }
                        inc++;
                    }
                }
                //draw
                Draw(element, ornament, position, invoker, false);
            }
        }

        public void Draw(FrameworkElement element, string ornament, string position, Invoker invoker, bool firstdraw)
        {
            TextBlock lab = new TextBlock();
            lab.Text = ornament;
            if (ornament.Length >5)
            {
                string gs = lab.Text.Substring(0, 5);
                //if ornament add to goup.
                if (gs == "groep" || gs =="group")
                {
                    Group lastgroup = invoker.selectedGroups.Last();
                    //group size variables.
                    double groupleft = 1000;
                    double grouptop = 1000;
                    double groupwidth = 0;
                    double groupheight = 0;
                    double groupright = 0;
                    double groupbottom = 0;
                    //calculate group size.
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
                        //lastgroup.ornamentPosition = "top";
                    }
                    else if (position == "bottom")
                    {
                        Canvas.SetLeft(lab, groupright - groupleft);
                        Canvas.SetTop(lab, grouptop + groupheight + 25);
                        //lastgroup.ornamentPosition = "bottom";
                    }
                    else if (position == "left")
                    {
                        Canvas.SetLeft(lab, groupleft - 25);
                        Canvas.SetTop(lab, groupbottom - grouptop);
                        //lastgroup.ornamentPosition = "left";
                    }
                    else if (position == "right")
                    {
                        Canvas.SetLeft(lab, groupleft + groupwidth + 25);
                        Canvas.SetTop(lab, groupbottom - grouptop);
                        //lastgroup.ornamentPosition = "right";
                    }
                    //add to canvas
                    lab.AccessKey = Convert.ToString(lastgroup.id);
                    Canvas parent = (Canvas)element.Parent;
                    parent.Children.Add(lab);
                }
                //else ornament add to element
                else
                {
                    //string ornamentPosition;
                    if (position == "top")
                    {
                        Canvas.SetLeft(lab, element.ActualOffset.X);
                        Canvas.SetTop(lab, element.ActualOffset.Y - 25);
                        //ornamentPosition = "top";
                    }
                    else if (position == "bottom")
                    {
                        Canvas.SetLeft(lab, element.ActualOffset.X);
                        Canvas.SetTop(lab, element.ActualOffset.Y + element.Height + 25);
                        //ornamentPosition = "bottom";
                    }
                    else if (position == "left")
                    {
                        Canvas.SetLeft(lab, element.ActualOffset.X - 25);
                        Canvas.SetTop(lab, element.ActualOffset.Y);
                        //ornamentPosition = "left";
                    }
                    else if (position == "right")
                    {
                        Canvas.SetLeft(lab, element.ActualOffset.X + element.Width + 25);
                        Canvas.SetTop(lab, element.ActualOffset.Y);
                        //ornamentPosition = "right";
                    }
                    //add to canvas
                    lab.AccessKey = Convert.ToString(element.AccessKey);
                    Canvas parent = (Canvas)element.Parent;
                    parent.Children.Add(lab);
                }
                //add to drawn
                if (firstdraw ==false)
                {
                    invoker.drawnOrnaments.Add(lab);
                }              

            }

        }

        public void Undraw(Invoker invoker, Canvas paintSurface)
        {
            TextBlock lastlab = invoker.drawnOrnaments.Last();
            invoker.removedOrnaments.Add(lastlab);
            invoker.drawnOrnaments.RemoveAt(invoker.drawnOrnaments.Count() -1);

            Repaint(invoker, paintSurface); //repaint
        }

        public void Redraw(Invoker invoker, Canvas paintSurface)
        {
            TextBlock lastlab = invoker.removedOrnaments.Last();
            invoker.drawnOrnaments.Add(lastlab);
            invoker.removedOrnaments.RemoveAt(invoker.removedOrnaments.Count() - 1);

            Repaint(invoker, paintSurface); //repaint
        }

        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            foreach (FrameworkElement drawornament in invoker.drawnOrnaments)
            {
                paintSurface.Children.Add(drawornament); //add
            }
        }

    }



    //public void SetOrnament()
    //{

    //}

    ////shape interface
    //public interface IShapes
    //{
    //    void Draw();
    //}

    ////rectangle shape from shape interface
    //public class RectangleShape : IShapes
    //{
    //    public void Draw()
    //    {

    //    }
    //}

    ////ellipse shape from shape interface
    //public class EllipseShape : IShapes
    //{
    //    public void Draw()
    //    {

    //    }
    //}

    ////abstract shape decorator
    //public abstract class ShapeDecorator : IShapes
    //{
    //    protected IShapes decoratedShape;

    //    public ShapeDecorator(IShapes decoratedShape)
    //    {
    //        this.decoratedShape = decoratedShape;
    //    }

    //    public virtual void Draw()
    //    {
    //        decoratedShape.Draw();
    //    }
    //}

    ////concrete decorator
    //public class OrnamentedDecorator : ShapeDecorator
    //{

    //    public OrnamentedDecorator(IShapes decoratedShape) : base(decoratedShape)
    //    //public OrnamentedDecorator(IShapes decoratedShape)
    //    //public override void OrnamentedDecorator()
    //    {
    //        //base(decoratedShape);
    //    }


    //   public override void Draw()
    //   {
    //        decoratedShape.Draw();
    //        //SetOrnament(decoratedShape,element,ornament,position,invoker);
    //        SetOrnament(decoratedShape);
    //   }

    //    //private void SetOrnament(IShapes decoratedShape, FrameworkElement element, string ornament, string position, Invoker invoker)
    //    private void SetOrnament(IShapes decoratedShape)
    //    {

    //    }
    //}



}
