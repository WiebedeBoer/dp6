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
                            Add(shape, position, ornament, invoker);
                        }
                        //inc++;
                    }
                }
                //draw
                Draw(element, ornament, position, invoker, false);
            }
        }

        //add ornaments
        public void Add(IDecoratorShape decoshape, string position, string ornament, Invoker invoker)
        {
            Shape shape = decoshape.Execute();
            shape.ornamentPositions.Add(position);
            shape.ornamentNames.Add(ornament);
            shape.ornamentKeyNames.Add(invoker.executer.ToString());
        }

        //prepare undo ornament
        public void PrepareOrnamentUndo(Invoker invoker)
        {
            List<TextBlock> PrepareUndo = new List<TextBlock>();
            foreach (TextBlock drawornament in invoker.drawnOrnaments)
            {
                PrepareUndo.Add(drawornament); //add
            }
            invoker.undoOrnamentsList.Add(PrepareUndo);
        }

        //draw ornaments
        public void Draw(FrameworkElement element, string ornament, string position, Invoker invoker, bool firstdraw)
        {
            //prepare undo
            PrepareOrnamentUndo(invoker);
            //draw
            TextBlock lab = new TextBlock();
            lab.Text = ornament;
            lab.Name = invoker.executer.ToString();
            if (ornament.Length >=5)
            {
                string gs = lab.Text.Substring(0, 5);
                //if ornament add to goup.
                if (gs == "groep" || gs == "group" || gs == "Groep" || gs == "Group")
                {
                    Group lastgroup = invoker.selectedGroups.Last(); //err
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
                        Canvas.SetLeft(lab, (((groupright - groupleft) / 2) + groupleft));
                        Canvas.SetTop(lab, grouptop - 25);
                    }
                    else if (position == "bottom" || position == "Bottom")
                    {
                        Canvas.SetLeft(lab, (((groupright - groupleft) / 2) + groupleft));
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
            List<TextBlock> lastUndo = invoker.undoOrnamentsList.Last();
            invoker.redoOrnamentsList.Add(lastUndo);
            invoker.undoOrnamentsList.RemoveAt(invoker.undoOrnamentsList.Count() - 1);
            //repaint
            paintSurface.Children.Clear();
            foreach (TextBlock drawornament in lastUndo)
            {
                paintSurface.Children.Add(drawornament); //add
            }
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }

            /*
            
            int num = invoker.drawnOrnaments.Count() - 1;
            Shape selectedShape = null;
            TextBlock element = null;
            int ornamentnum = -1;
            //shapes
            for (int inc = num; inc >=0; inc--)
            {
                element = invoker.drawnOrnaments[inc];
                string nkey = element.Name;
                string akey = element.AccessKey;
                foreach (Shape drawnShape in invoker.drawnShapes)
                {
                    foreach (String drawnName in drawnShape.ornamentKeyNames)
                    {
                        if (drawnName == nkey)
                        {
                            selectedShape = drawnShape;
                            ornamentnum = inc;
                            

                            //string gs = element.Text.Substring(0, 5);
                            ////if ornament of goup.
                            //if (gs == "groep" || gs == "group" || gs == "Groep" || gs == "Group")
                            //{
                            //    invoker.undoGroupOrnaments.Add(element);
                            //}
                            inc = -1;
                        }
                    }
                }

                
        }

            if (selectedShape != null)
            {
                RemoveFromShape(selectedShape, invoker);
            }

            Group selectedGroup = null;
            for (int ing = num; ing >= 0; ing--)
            {
                element = invoker.drawnOrnaments[ing];
                string nkey = element.Name;
                string akey = element.AccessKey;
                foreach (Group drawnGroup in invoker.drawnGroups)
                {
                    foreach (String drawnName in drawnGroup.ornamentKeyNames)
                    {
                        if (drawnName == nkey)
                        {
                            selectedGroup = drawnGroup;
                            ornamentnum = ing;
                            ing = -1;
                        }
                    }
                }
            }

            if (selectedGroup != null)
            {
                RemoveFromGroup(selectedGroup, invoker);
            }

            if (ornamentnum >=0)
            {
                invoker.undoOrnaments.Add(element);
                invoker.drawnOrnaments.RemoveAt(ornamentnum);
            }



            Repaint(invoker, paintSurface); //repaint

            */
        }

        //re add ornament
        public void Redraw(Invoker invoker, Canvas paintSurface)
        {
            List<TextBlock> lastRedo = invoker.redoOrnamentsList.Last();
            invoker.undoOrnamentsList.Add(lastRedo);
            invoker.redoOrnamentsList.RemoveAt(invoker.redoOrnamentsList.Count() - 1);
            //repaint
            paintSurface.Children.Clear();
            foreach (TextBlock drawornament in lastRedo)
            {
                paintSurface.Children.Add(drawornament); //add
            }
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            /*
            
            int num = invoker.undoOrnaments.Count() - 1;
            Shape selectedShape = null;
            TextBlock element = null;
            int ornamentnum = -1;
            for (int inc = num; inc >= 0; inc--)
            {
                element = invoker.undoOrnaments[inc];
                string nkey = element.Name;
                string akey = element.AccessKey;
                foreach (Shape drawnShape in invoker.drawnShapes)
                {
                    foreach (String drawnName in drawnShape.undoKeyNames)
                    {
                        if (drawnName == nkey)
                        {
                            selectedShape = drawnShape;
                            ornamentnum = inc;
                            inc = -1;
                        }
                    }
                }
            }

            if (selectedShape != null)
            {
                AddToShape(selectedShape, invoker);
            }

            Group selectedGroup = null;
            for (int ing = num; ing >= 0; ing--)
            {
                element = invoker.undoOrnaments[ing];
                string nkey = element.Name;
                string akey = element.AccessKey;
                foreach (Group drawnGroup in invoker.drawnGroups)
                {
                    foreach (String drawnName in drawnGroup.undoKeyNames)
                    {
                        if (drawnName == nkey)
                        {
                            selectedGroup = drawnGroup;
                            ornamentnum = ing;
                            ing = -1;
                        }
                    }
                }
            }

            if (selectedGroup != null)
            {
                RemoveFromGroup(selectedGroup, invoker);
            }

            if (ornamentnum >= 0)
            {
                invoker.drawnOrnaments.Add(element);
                invoker.undoOrnaments.RemoveAt(ornamentnum);
            }
            Repaint(invoker, paintSurface); //repaint

            */
        }



        //remove names for repaint
        public void RemoveFromShape(Shape selectedShape, Invoker invoker)
        {
            //for repaint
            String lastornname = selectedShape.ornamentNames.Last();
            selectedShape.ornamentNames.RemoveAt(selectedShape.ornamentNames.Count() - 1);
            selectedShape.undoOrnamentNames.Add(lastornname);
            //for redo
            String lastname = selectedShape.ornamentKeyNames.Last();
            selectedShape.ornamentKeyNames.RemoveAt(selectedShape.ornamentKeyNames.Count() - 1);
            selectedShape.undoKeyNames.Add(lastname);
        }

        public void RemoveFromGroup(Group selectedGroup, Invoker invoker)
        {
            //for repaint
            String lastornname = selectedGroup.ornamentNames.Last();
            selectedGroup.ornamentNames.RemoveAt(selectedGroup.ornamentNames.Count() - 1);
            selectedGroup.undoOrnamentNames.Add(lastornname);
            //for redo
            String lastname = selectedGroup.ornamentKeyNames.Last();
            selectedGroup.ornamentKeyNames.RemoveAt(selectedGroup.ornamentKeyNames.Count() - 1);
            selectedGroup.undoKeyNames.Add(lastname);
        }

        //re add names for repaint
        public void AddToShape(Shape selectedShape, Invoker invoker)
        {
            //for repaint
            String lastornname = selectedShape.undoOrnamentNames.Last();
            selectedShape.undoOrnamentNames.RemoveAt(selectedShape.undoOrnamentNames.Count() - 1);
            selectedShape.ornamentNames.Add(lastornname);
            //for undo
            String lastname = selectedShape.undoKeyNames.Last();
            selectedShape.undoKeyNames.RemoveAt(selectedShape.undoKeyNames.Count() - 1);
            selectedShape.ornamentKeyNames.Add(lastname);
        }

        //re add names for repaint
        public void AddToGroup(Shape selectedGroup, Invoker invoker)
        {
            //for repaint
            String lastornname = selectedGroup.undoOrnamentNames.Last();
            selectedGroup.undoOrnamentNames.RemoveAt(selectedGroup.undoOrnamentNames.Count() - 1);
            selectedGroup.ornamentNames.Add(lastornname);
            //for undo
            String lastname = selectedGroup.undoKeyNames.Last();
            selectedGroup.undoKeyNames.RemoveAt(selectedGroup.undoKeyNames.Count() - 1);
            selectedGroup.ornamentKeyNames.Add(lastname);
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

                //foreach (FrameworkElement drawornament in invoker.drawnOrnaments)
                //{
                //    paintSurface.Children.Add(drawornament); //add
                //}
                foreach (Shape shape in invoker.drawnShapes)
                {
                    int i = 0;
                    foreach (string ornament in shape.ornamentNames)
                    {
                        OrnamentDecorator deco = new OrnamentDecorator(shape);
                        deco.Draw(shape.madeelement, ornament, shape.ornamentPositions[i], invoker, false);
                        i++;
                    }
                }

            foreach (Group group in invoker.drawnGroups)
            {
                int i = 0;
                foreach (string ornament in group.ornamentNames)
                {
                    if (group.drawnShapes.Count() > 0)
                    {
                        OrnamentDecorator deco = new OrnamentDecorator(group);
                        Shape groupshape = group.drawnShapes.First();
                        deco.Draw(groupshape.madeelement, ornament, group.ornamentPositions[i], invoker, false);
                    }

                    i++;
                }
            }

        }

        ////remove selected ornament by name key
        //public void RemoveOrnament(Invoker invoker)
        //{

        //    int inc = 0;
        //    //int number = 0;

        //    string key = element.Name;
        //    string akey = element.AccessKey;
        //    Shape selectedShape = null;
        //    foreach (Shape drawnShape in invoker.drawnShapes)
        //    {

        //        foreach (String drawnName in drawnShape.ornamentNames)
        //        if (drawnName == key)
        //        {
        //            //number = inc;
        //            selectedShape = drawnShape;
        //        }
        //        //inc++;
        //    }
        //    //invoker.drawnElements.RemoveAt(number);
        //    //invoker.removedElements.Add(element);
        //    //invoker.movedElements.Add(element);
        //    if (selectedShape !=null)
        //    {
        //        RemoveFromShape(selectedShape, invoker);
        //    }

        //}

        ////add selected ornament by name key
        //public void AddOrnament(Invoker invoker)
        //{
        //    string key = element.Name;
        //    string akey = element.AccessKey;
        //    //int inc = 0;
        //    //int number = 0;
        //    Shape selectedShape = null;
        //    foreach (Shape drawnShape in invoker.drawnShapes)
        //    {

        //        foreach (String drawnName in drawnShape.undoKeyNames)
        //            if (drawnName == key)
        //            {
        //                //number = inc;
        //                selectedShape = drawnShape;
        //            }
        //        //inc++;
        //    }
        //    //invoker.drawnElements.RemoveAt(number);
        //    //invoker.removedElements.Add(element);
        //    //invoker.movedElements.Add(element);
        //    if (selectedShape != null)
        //    {
        //        AddToShape(selectedShape, invoker);
        //    }

        //}


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
