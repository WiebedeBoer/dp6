using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace tekenprogramma
{

    public abstract class Decorator : IDecoratorShape
    {
        protected IDecoratorShape decoshape;

        //public Decorator(double x, double y, double width, double height) : base(x, y, width, height)
        //{

        //}

        public Decorator(IDecoratorShape decoshape) 
        {
            this.decoshape = decoshape;
        }

        public Shape Execute()
        {
            if (decoshape != null)
            {
                Shape shape = decoshape.Execute();
                return shape;
            }
            else
            {
                return null;
            }
        }

        public Group Fetch()
        {
            if (decoshape != null)
            {
                Group group = decoshape.Fetch();
                return group;
            }
            else
            {
                return null;
            }        
        }

        public void Draw() 
        {
            if (decoshape != null)
            {
                decoshape.Draw();
            }
        }
        public void Add(string position, string name) 
        {
            if (decoshape != null)
            {
                decoshape.Add(position, name);
            }
        }
    }

    public class OrnamentDecorator : Decorator
    {

        //: base(x, y, width, height)
        //public OrnamentDecorator() 
        //{

        //}

        public OrnamentDecorator(IDecoratorShape decoshape) : base(decoshape)
        {

        }


        public void SetOrnament(FrameworkElement element, string ornament, string position, Invoker invoker)
        {
            if (ornament.Length > 5 && (position == "top" || position == "bottom" || position == "left" || position == "right" || position == "Top" || position == "Bottom" || position == "Left" || position == "Right"))
            {
                string gs = ornament.Substring(0, 5);
                //if ornament add to goup
                if (gs == "groep" || gs == "group" || gs == "Groep" || gs == "Group")
                {
                    Group lastgroup = invoker.selectedGroups.Last();
                    lastgroup.Add(position, ornament);
                }
                //else ornament add to element
                else
                {
                    //int inc = 0;
                    //int num = 0;
                    //foreach (IComponent component in invoker.drawnComponents)
                    foreach (Shape shape in invoker.drawnShapes)
                    {
                        FrameworkElement fetched = shape.madeelement;
                        if (fetched.AccessKey == element.AccessKey)
                        {
                            //num = inc;
                            //component.Add(shape, position, ornament);
                            Add(shape, position, ornament);
                        }
                        //inc++;
                    }
                }
                //draw
                Draw(element, ornament, position, invoker, false);
            }
        }

        public void Add(IDecoratorShape decoshape, string position, string ornament)
        {
            Shape shape = decoshape.Execute();
            shape.ornamentPositions.Add(position);
            shape.ornamentNames.Add(ornament);
        }

        public void Draw(FrameworkElement element, string ornament, string position, Invoker invoker, bool firstdraw)
        {
            TextBlock lab = new TextBlock();
            lab.Text = ornament;
            lab.Name = invoker.executer.ToString();
            if (ornament.Length >=5)
            {
                string gs = lab.Text.Substring(0, 5);
                //if ornament add to goup.
                if (gs == "groep" || gs == "group" || gs == "Groep" || gs == "Group")
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
                    if (position == "top" || position == "Top")
                    {
                        Canvas.SetLeft(lab, groupright - groupleft);
                        Canvas.SetTop(lab, grouptop - 25);
                    }
                    else if (position == "bottom" || position == "Bottom")
                    {
                        Canvas.SetLeft(lab, groupright - groupleft);
                        Canvas.SetTop(lab, grouptop + groupheight + 25);
                    }
                    else if (position == "left" || position == "Left")
                    {
                        Canvas.SetLeft(lab, groupleft - 25);
                        Canvas.SetTop(lab, groupbottom - grouptop);
                    }
                    else if (position == "right" || position == "Right")
                    {
                        Canvas.SetLeft(lab, groupleft + groupwidth + 25);
                        Canvas.SetTop(lab, groupbottom - grouptop);
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
                    if (position == "top" || position =="Top")
                    {
                        Canvas.SetLeft(lab, element.ActualOffset.X);
                        Canvas.SetTop(lab, element.ActualOffset.Y - 25);
                    }
                    else if (position == "bottom" || position == "Bottom")
                    {
                        Canvas.SetLeft(lab, element.ActualOffset.X);
                        Canvas.SetTop(lab, element.ActualOffset.Y + element.Height + 25);
                    }
                    else if (position == "left" || position == "Left")
                    {
                        Canvas.SetLeft(lab, element.ActualOffset.X - 25);
                        Canvas.SetTop(lab, element.ActualOffset.Y);
                    }
                    else if (position == "right" || position == "Right")
                    {
                        Canvas.SetLeft(lab, element.ActualOffset.X + element.Width + 25);
                        Canvas.SetTop(lab, element.ActualOffset.Y);
                    }
                    //add to canvas
                    lab.AccessKey = Convert.ToString(element.AccessKey);
                    Canvas parent = (Canvas)element.Parent;
                    parent.Children.Add(lab); //err
                }
                //add to drawn
                if (firstdraw ==false)
                {
                    invoker.drawnOrnaments.Add(lab);
                }              

            }

        }

        //remove ornament
        public void Undraw(Invoker invoker, Canvas paintSurface)
        {
            TextBlock lastlab = invoker.drawnOrnaments.Last();
            invoker.removedOrnaments.Add(lastlab);
            invoker.drawnOrnaments.RemoveAt(invoker.drawnOrnaments.Count() -1);
            Repaint(invoker, paintSurface); //repaint
        }

        //re add ornament
        public void Redraw(Invoker invoker, Canvas paintSurface)
        {
            TextBlock lastlab = invoker.removedOrnaments.Last();
            invoker.drawnOrnaments.Add(lastlab);
            invoker.removedOrnaments.RemoveAt(invoker.removedOrnaments.Count() - 1);
            Repaint(invoker, paintSurface); //repaint
        }

        //
        //repaint
        //

        //repaint
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
            //foreach (Shape shape in invoker.drawnShapes)
            //{
            //    int i = 0;
            //    foreach (string ornament in shape.ornamentNames)
            //    {
            //        OrnamentDecorator deco = new OrnamentDecorator(shape);
            //        deco.Draw(shape.madeelement, ornament, shape.ornamentPositions[i], invoker, false);
            //        i++;
            //    }
            //}
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
