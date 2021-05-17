using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace tekenprogramma
{

    //location class
    public class Location
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public Location(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Location()
        {

        }
    }

    //class group
    public class Group : IDecoratorShape
    {
        public double height;
        public double width;
        public double x;
        public double y;
        public string type;
        public int depth;
        public int id;
        //public List<Baseshape> groupitems;
        public Canvas selectedCanvas;

        //public List<Baseshape> children = new List<Baseshape>();

        //public List<FrameworkElement> selectedElements = new List<FrameworkElement>();
        //public List<FrameworkElement> unselectedElements = new List<FrameworkElement>();

        public List<FrameworkElement> drawnElements = new List<FrameworkElement>();
        public List<FrameworkElement> removedElements = new List<FrameworkElement>();
        public List<FrameworkElement> movedElements = new List<FrameworkElement>();
        public List<FrameworkElement> unmovedElements = new List<FrameworkElement>();
        //public List<FrameworkElement> selectElementsList = new List<FrameworkElement>();
        //public List<FrameworkElement> unselectElementsList = new List<FrameworkElement>();

        public List<Shape> drawnShapes = new List<Shape>();
        //public List<Shape> removedShapes = new List<Shape>();
        //public List<Shape> movedShapes = new List<Shape>();
        //public List<Shape> selectShapes = new List<Shape>();
        //public List<Shape> unselectShapes = new List<Shape>();

        public List<Group> addedGroups = new List<Group>();
        public List<Group> removedGroups = new List<Group>();
        public List<Group> movedGroups = new List<Group>();
        public List<Group> unmovedGroups = new List<Group>();
        public List<Group> selectedGroups = new List<Group>();
        public List<Group> unselectedGroups = new List<Group>();

        public List<IComponent> drawnComponents = new List<IComponent>();
        public List<IComponent> removedComponents = new List<IComponent>();
        public List<IComponent> movedComponents = new List<IComponent>();

        public Invoker invoker;
        public FrameworkElement element;
        public Canvas lastCanvas;

        //public TextBlock ornament = new TextBlock();
        //public string ornamentPosition = null;

        public List<TextBlock> ornaments = new List<TextBlock>();
        public List<TextBlock> removedOrnaments = new List<TextBlock>();
        public List<string> ornamentNames = new List<string>();
        public List<string> remvedOrnamentNames = new List<string>();
        public List<string> ornamentPositions = new List<string>();
        public List<string> removedOrnamentPositions = new List<string>();

        public Group(double x, double y, double width, double height, string type, int depth, int id, Canvas selectedCanvas, Invoker invoker, FrameworkElement element)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            this.type = type;
            this.depth = depth;
            this.id = id;
            this.selectedCanvas = selectedCanvas;
            this.invoker = invoker;
            this.element = element;
        }

        public Group Fetch()
        {
            return this;
        }
        public void Draw() { }

        public Shape Execute()
        {
            Shape shape = this.drawnShapes.Last();
            return shape;
        }

        //public void Add(TextBlock lab, string position, string name)
        public void Add(string position, string name)
        {
            //this.ornaments.Add(lab);
            this.ornamentPositions.Add(position);
            this.ornamentNames.Add(name);
        }

        public void Remove()
        {
            //this.ornaments.RemoveAt(this.ornaments.Count() - 1);
            this.ornamentPositions.RemoveAt(this.ornamentPositions.Count() - 1);
            this.ornamentNames.RemoveAt(this.ornamentNames.Count() - 1);
        }

        //make new group
        public void MakeGroup(Group group, Canvas selectedCanvas, Invoker invoker)
        {
            //depth
            int newdepth = 0;
            if (invoker.selectElementsList.Count() > 0)
            {
                newdepth = 1;
                //get selected elements
                foreach (FrameworkElement elm in invoker.selectElementsList)
                //for (int index = 0; index < invoker.selectElementsList.Count(); index++)
                {
                    //if (invoker.selectElementsList.Count() > 0)
                    //{
                    //FrameworkElement elm = invoker.selectElementsList[index];
                    elm.Opacity = 0.8;
                    //check if selected is not already grouped element
                    int elmcheck = CheckInGroup(invoker, elm);
                    if (elmcheck == 0)
                    {
                        this.drawnElements.Add(elm);
                        //add components
                        if (elm.Name == "Rectangle")
                        {
                            Strategy component = ConcreteComponentRectangle.GetInstance();
                            //IComponent rectangle = new ConcreteComponentRectangle(elm.ActualOffset.X, elm.ActualOffset.Y, elm.Width, elm.Height);
                            //this.drawnComponents.Add(rectangle);
                            this.drawnComponents.Add(component);
                            //Shape shape = new Shape(elm.ActualOffset.X, elm.ActualOffset.Y, elm.Width, elm.Height);
                            Shape shape = SelectInShape(elm);
                            this.drawnShapes.Add(shape);
                        }
                        else if (elm.Name == "Ellipse")
                        {
                            Strategy component = ConcreteComponentEllipse.GetInstance();
                            //IComponent ellipse = new ConcreteComponentEllipse(elm.ActualOffset.X, elm.ActualOffset.Y, elm.Width, elm.Height);
                            //this.drawnComponents.Add(ellipse);
                            this.drawnComponents.Add(component);
                            //Shape shape = new Shape(elm.ActualOffset.X, elm.ActualOffset.Y, elm.Width, elm.Height);
                            Shape shape = SelectInShape(elm);
                            this.drawnShapes.Add(shape);
                        }

                    }
                    //remove selected
                    invoker.unselectElementsList.Add(elm);
                    //invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                    //}


                }
                //clear selected elements
                invoker.selectElementsList.Clear();
            }
            //add selected groups
            if (invoker.selectedGroups.Count() > 0)
            {
                newdepth = 1;
                //get selected groups
                foreach (Group selectedgroup in invoker.selectedGroups)
                {
                    this.addedGroups.Add(selectedgroup);
                    //remove selected
                    invoker.unselectedGroups.Add(selectedgroup);
                    //invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                    SelectedGroup(selectedgroup, invoker); //remove from drawn groups
                    if (selectedgroup.depth > newdepth)
                    {
                        newdepth = newdepth + selectedgroup.depth;
                    }
                }
                //clear selected groups
                invoker.selectedGroups.Clear();                
            }
            this.depth = newdepth;
            this.id = invoker.executer; //id
            invoker.drawnGroups.Add(this);
            //this.id = invoker.executer; //id

        }

        //removed selected group from drawn elements
        public void SelectedGroup(Group group, Invoker invoker)
        {
            int key = group.id;
            int inc = 0;
            int number = 0;
            foreach (Group drawn in invoker.drawnGroups)
            {
                if (drawn.id == key)
                {
                    number = inc;
                }
                inc++;
            }
            //if ()
            //{

            //}
            invoker.drawnGroups.RemoveAt(number);
        }

        //un group
        public void UnGroup(Canvas selectedCanvas, Invoker invoker)
        {
            Group lastgroup = invoker.drawnGroups.Last();
            invoker.drawnGroups.RemoveAt(invoker.drawnGroups.Count() - 1);
            if (lastgroup.drawnElements.Count() > 0)
            {
                //get elements
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    //add selected
                    invoker.selectElementsList.Add(elm);
                    if (invoker.unselectElementsList.Count() > 0)
                    {
                        invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1);
                    }
                    //elm.Opacity = 0.5;
                }
            }
            if (lastgroup.addedGroups.Count() > 0)
            {
                //get groups
                foreach (Group selectedgroup in lastgroup.addedGroups)
                {
                    //add selected
                    invoker.selectedGroups.Add(selectedgroup);
                    if (invoker.unselectedGroups.Count() > 0)
                    {
                        invoker.unselectedGroups.RemoveAt(invoker.unselectedGroups.Count() - 1);
                    }
                    invoker.drawnGroups.Add(selectedgroup); //re add to drawn
                }
            }
            invoker.removedGroups.Add(lastgroup);
        }

        //re group
        public void ReGroup(Canvas selectedCanvas, Invoker invoker)
        {
            Group lastgroup = invoker.removedGroups.Last();

            if (lastgroup.drawnElements.Count() > 0)
            {
                //get elements
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    //remove selected
                    invoker.unselectElementsList.Add(elm);
                    invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                }
            }
            if (lastgroup.addedGroups.Count() > 0)
            {
                //get groups
                foreach (Group selectedgroup in lastgroup.addedGroups)
                {
                    //remove selected
                    invoker.unselectedGroups.Add(selectedgroup);
                    invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                    invoker.drawnGroups.RemoveAt(invoker.drawnGroups.Count() - 1);
                }
            }
            invoker.drawnGroups.Add(lastgroup);
            invoker.removedGroups.RemoveAt(invoker.removedGroups.Count() - 1);
        }

        //
        //moving
        //

        //moving
        public void MoveClient(Group selectedgroup, IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            List<IComponent> components = selectedgroup.drawnComponents;
            List<FrameworkElement> drawnElements = selectedgroup.drawnElements;
            
            invoker.unmovedGroups.Add(selectedgroup);
            //invoker.removedGroups.Add(selectedgroup);
            //calculate difference in location
            double leftOffset = Convert.ToDouble(selectedelement.ActualOffset.X) - e.GetCurrentPoint(paintSurface).Position.X;
            double topOffset = Convert.ToDouble(selectedelement.ActualOffset.Y) - e.GetCurrentPoint(paintSurface).Position.Y;
            if (selectedgroup.drawnElements.Count() > 0)
            {
                int i = 0;
                foreach (var component in components)
                {
                    FrameworkElement movedElement = drawnElements[i];
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X) - leftOffset;
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y) - topOffset;
                    location.width = movedElement.Width;
                    location.height = movedElement.Height;
                    invoker.executer++;//acceskey add
                    i++;
                    FrameworkElement madeElement = component.Accept(visitor, invoker, e, paintSurface, movedElement, location);
                    selectedgroup.movedElements.Add(madeElement);
                }
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    MoveClient submover = new MoveClient();
                    IVisitor subvisitor = new ConcreteVisitorMove();
                    submover.Client(subgroup.drawnComponents, subgroup.drawnElements, subgroup, subvisitor, invoker, e, paintSurface, selectedelement);
                }
            }
            //add to moved or resized
            invoker.movedGroups.Add(selectedgroup);
            //remove selected group
            invoker.unselectedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);

            selectedgroup.Repaint(invoker, paintSurface); //repaint
        }


        //
        //undo redo move resize
        //

        //undo moving or resizing
        public void Undo(Invoker invoker, Canvas paintSurface)
        {

            Group selectedgroup = invoker.unmovedGroups.Last();
            invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            invoker.unmovedGroups.Add(selectedgroup);
            invoker.selectedGroups.Add(selectedgroup); //re add to selected

            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                }
            }
            if (selectedgroup.movedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.movedGroups)
                {
                    selectedgroup.SubUndo(subgroup, invoker);
                }
            }
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.drawnElements)
                {
                    //invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                }
            }
            Repaint(invoker, paintSurface); //repaint 

            //Group selectedgroup = invoker.unmovedGroups.Last();
            //invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            //invoker.unmovedGroups.Add(selectedgroup);
            //invoker.selectedGroups.Add(selectedgroup); //re add to selected

            //if (selectedgroup.drawnElements.Count() > 0)
            //{
            //    foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
            //    {
            //        invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            //        //invoker.removedElements.Add(movedElement);
            //    }
            //}
            //if (selectedgroup.movedGroups.Count() > 0)
            //{
            //    foreach (Group subgroup in selectedgroup.movedGroups)
            //    {
            //        selectedgroup.SubUndo(subgroup, invoker);
            //    }
            //}
            //if (selectedgroup.drawnElements.Count() > 0)
            //{
            //    foreach (FrameworkElement removedElement in selectedgroup.drawnElements)
            //    {
            //        //invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
            //        invoker.drawnElements.Add(removedElement);
            //    }
            //}
            //Repaint(invoker, paintSurface); //repaint 

            //Group selectedgroup = invoker.removedGroups.Last();
            //invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            //invoker.removedGroups.Add(selectedgroup);
            //invoker.selectedGroups.Add(selectedgroup); //re add to selected

            //if (selectedgroup.drawnElements.Count() > 0)
            //{
            //    foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
            //    {
            //        invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            //        //invoker.removedElements.Add(movedElement);
            //    }
            //}
            //if (selectedgroup.movedGroups.Count() > 0)
            //{
            //    foreach (Group subgroup in selectedgroup.movedGroups)
            //    {
            //        selectedgroup.SubUndo(subgroup, invoker);
            //    }
            //}
            //if (selectedgroup.drawnElements.Count() > 0)
            //{
            //    foreach (FrameworkElement removedElement in selectedgroup.drawnElements)
            //    {
            //        //invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
            //        invoker.drawnElements.Add(removedElement);
            //    }
            //}
            //Repaint(invoker, paintSurface); //repaint   
        }

        public void SubUndo(Group selectedgroup, Invoker invoker)
        {
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                }
            }
            if (selectedgroup.movedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.movedGroups)
                {
                    selectedgroup.SubUndo(subgroup, invoker);
                }
            }
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.drawnElements)
                {
                    //invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                }
            }
        }

        //redo moving or resizing
        public void Redo(Invoker invoker, Canvas paintSurface)
        {

            Group selectedgroup = invoker.unmovedGroups.Last();
            invoker.unmovedGroups.RemoveAt(invoker.unmovedGroups.Count() - 1);
            invoker.movedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1); //remove selected

            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.movedElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                    //invoker.movedElements.Add(movedElement);              
                }
            }
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubRedo(subgroup, invoker);
                }
            }
            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.movedElements)
                //foreach (FrameworkElement removedElement in selectedgroup.removedElements)
                {
                    //invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                    //invoker.movedElements.Add(removedElement);
                }
            }
            Repaint(invoker, paintSurface); //repaint 
            
            //Group selectedgroup = invoker.removedGroups.Last();
            //invoker.removedGroups.RemoveAt(invoker.removedGroups.Count() - 1);
            //invoker.movedGroups.Add(selectedgroup);
            //invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1); //remove selected

            //if (selectedgroup.movedElements.Count() > 0)
            //{
            //    foreach (FrameworkElement movedElement in selectedgroup.movedElements)
            //    {
            //        invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            //        //invoker.removedElements.Add(movedElement);
            //        //invoker.movedElements.Add(movedElement);              
            //    }
            //}
            //if (selectedgroup.addedGroups.Count() > 0)
            //{
            //    foreach (Group subgroup in selectedgroup.addedGroups)
            //    {
            //        selectedgroup.SubRedo(subgroup, invoker);
            //    }
            //}
            //if (selectedgroup.movedElements.Count() > 0)
            //{
            //    foreach (FrameworkElement removedElement in selectedgroup.movedElements)
            //    //foreach (FrameworkElement removedElement in selectedgroup.removedElements)
            //    {
            //        //invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
            //        invoker.drawnElements.Add(removedElement);
            //        //invoker.movedElements.Add(removedElement);
            //    }
            //}
            //Repaint(invoker, paintSurface); //repaint   
        }

        public void SubRedo(Group selectedgroup, Invoker invoker)
        {
            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.movedElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                    //invoker.movedElements.Add(movedElement);
                }
            }
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubRedo(subgroup, invoker);
                }
            }
            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.movedElements)
                //foreach (FrameworkElement removedElement in selectedgroup.removedElements)
                {
                    //invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                    //invoker.movedElements.Add(removedElement);
                }
            }
        }

        //
        //resizing
        //

        //resizing
        public void ResizeClient(Group selectedgroup, IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            List<IComponent> components = selectedgroup.drawnComponents;
            List<FrameworkElement> drawnElements = selectedgroup.drawnElements;
            invoker.unmovedGroups.Add(selectedgroup);
            //invoker.removedGroups.Add(selectedgroup);
            //calculate difference in size
            double newWidth = selectedgroup.ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.X, Convert.ToDouble(selectedelement.ActualOffset.X));
            double newHeight = selectedgroup.ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.Y, Convert.ToDouble(selectedelement.ActualOffset.Y));
            double widthOffset = selectedelement.Width - newWidth;
            double heightOffset = selectedelement.Height - newHeight;

            if (selectedgroup.drawnElements.Count() > 0)
            {
                int i = 0;
                foreach (var component in components)
                {
                    FrameworkElement movedElement = drawnElements[i];
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X);
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y);
                    location.width = Convert.ToDouble(movedElement.Width) - widthOffset;
                    location.height = Convert.ToDouble(movedElement.Height) - heightOffset;
                    invoker.executer++; //acceskey add
                    i++;
                    FrameworkElement madeElement = component.Accept(visitor, invoker, e, paintSurface, movedElement, location);
                    selectedgroup.movedElements.Add(madeElement);
                }
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    ResizeClient subresizer = new ResizeClient();
                    IVisitor subvisitor = new ConcreteVisitorResize();
                    subresizer.Client(subgroup.drawnComponents, subgroup.drawnElements, subgroup, subvisitor, invoker, e, paintSurface, selectedelement);
                }
            }
            //add to moved or resized
            invoker.movedGroups.Add(selectedgroup);
            //remove selected group
            invoker.unselectedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);

            selectedgroup.Repaint(invoker, paintSurface);//repaint
        }

        //
        //other
        //

        //repaint
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
        }

        //
        //selecting
        //

        //get the selected shape
        public Shape SelectInShape(FrameworkElement selectedElement)
        {
            foreach (Shape shape in invoker.drawnShapes)
            {
                if (shape.madeelement.AccessKey == selectedElement.AccessKey)
                {
                    return shape;
                }
            }
            return null;
        }

        //check if element is already in group
        public int CheckInGroup(Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            foreach (Group group in invoker.drawnGroups)
            {
                if (group.drawnElements.Count() > 0)
                {
                    foreach (FrameworkElement groupelement in group.drawnElements)
                    {
                        if (groupelement.AccessKey == element.AccessKey)
                        {
                            counter++;
                        }
                    }
                }
                CheckInSubgroup(group, element.AccessKey);
            }
            return counter;
        }

        public int CheckInSubgroup(Group group, string key)
        {
            int counter = 0;
            if (group.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement groupelement in group.drawnElements)
                {
                    if (groupelement.AccessKey == element.AccessKey)
                    {
                        counter++;
                    }
                }
            }
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    counter = subgroup.CheckInSubgroup(subgroup, key);
                }
            }
            return counter;
        }

        //see if element is in group and select the group
        public bool SelectInGroup(FrameworkElement selectedElement, Invoker invoker)
        {
            string key = selectedElement.AccessKey;
            bool ingroup = false;
            if (invoker.drawnGroups.Count() > 0)
            {
                foreach (Group group in invoker.drawnGroups)
                {
                    if (group.drawnElements.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in group.drawnElements)
                        {
                            if (drawn.AccessKey == key)
                            {
                                invoker.selectedGroups.Add(group);
                                ingroup = true;
                                
                                ////remove selected element from list if in group
                                //if (invoker.selectElementsList.Count() >0)
                                //{
                                //    invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                                //}                         
                            }
                        }
                    }
                    SelectInGroupHandle(invoker, key, group); //subgroup recursive
                }
            }
            return ingroup;
        }

        //recursively see if element is in subgroup and select the group
        public void SelectInGroupHandle(Invoker invoker, string key, Group group)
        {
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    subgroup.SelectInGroupHandle(invoker, key, group);
                    foreach (FrameworkElement drawn in subgroup.drawnElements)
                    {
                        if (drawn.AccessKey == key)
                        {
                            invoker.selectedGroups.Add(group);
                        }
                    }
                }
            }
        }

        //set group unselected
        public void UnselectGroup(FrameworkElement selectedElement, Invoker invoker)
        {
            string key = selectedElement.AccessKey;
            if (invoker.drawnGroups.Count() > 0)
            {
                foreach (Group group in invoker.drawnGroups)
                {
                    if (group.drawnElements.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in group.drawnElements)
                        {
                            if (drawn.AccessKey == key)
                            {
                                invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                            }
                        }
                    }
                    UnselectGroupHandle(invoker, key, group); //subgroup recursive
                }
            }
        }

        //set subgroup unselected
        public void UnselectGroupHandle(Invoker invoker, string key, Group group)
        {
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    subgroup.SelectInGroupHandle(invoker, key, group);
                    foreach (FrameworkElement drawn in subgroup.drawnElements)
                    {
                        if (drawn.AccessKey == key)
                        {
                            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                        }
                    }
                }
            }
        }

        //
        //miscellaneous
        //

        //remove selected element by access key
        public void KeyNumber(FrameworkElement element, Invoker invoker)
        {
            string key = element.AccessKey;
            int inc = 0;
            int number = 0;
            foreach (FrameworkElement drawn in invoker.drawnElements)
            {
                if (drawn.AccessKey == key)
                {
                    number = inc;
                }
                inc++;
            }
            invoker.drawnElements.RemoveAt(number);
            invoker.removedElements.Add(element);
            invoker.movedElements.Add(element);
        }

        //give smallest number
        public double ReturnSmallest(double first, double last)
        {
            if (first < last)
            {
                return last - first;
            }
            else
            {
                return first - last;
            }
        }

        //
        //loading
        //

        //loading
        public void LoadGroup(Group grouping, Canvas paintSurface, Invoker invoker, int linenumber, int start, int stop, string text)
        {
            string[] readText = Regex.Split(text, "\\n+");
            FrameworkElement elm = null;
            int i = start;
            while (i < stop)
            {
                string s = readText[i];
                string[] line = Regex.Split(s, "\\s+");
                if (line[0] == "ellipse")
                {
                    i++;
                    GetEllipse(s, paintSurface, invoker);
                    elm = invoker.drawnElements.Last();
                    grouping.drawnElements.Add(elm);
                }
                else if (line[0] == "rectangle")
                {
                    i++;
                    GetRectangle(s, paintSurface, invoker);
                    elm = invoker.drawnElements.Last();
                    grouping.drawnElements.Add(elm);
                }
                else if (line[0] == "group")
                {
                    i++;
                    invoker.executer++;
                    Group subgrouping = new Group(0, 0, 0, 0, "group", 0, invoker.executer, paintSurface, invoker, elm);
                    LoadGroup(subgrouping, paintSurface, invoker, Convert.ToInt32(line[1]), i, i + Convert.ToInt32(line[1]), text);
                    grouping.addedGroups.Add(subgrouping);
                }
            }
            invoker.drawnGroups.Add(grouping);
        }

        //load ellipse
        public void GetEllipse(String lines, Canvas paintSurface, Invoker invoker)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position
            paintSurface.Children.Add(newEllipse);
            invoker.drawnElements.Add(newEllipse);
        }

        //load rectangle
        public void GetRectangle(String lines, Canvas paintSurface, Invoker invoker)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            paintSurface.Children.Add(newRectangle);
            invoker.drawnElements.Add(newRectangle);
        }








        /*
        //moving element in group
        public FrameworkElement MovingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            KeyNumber(element, invoker); //move selected at removed
            //create at new location
            if (element.Name == "Rectangle")
            {
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = location.width; //set width
                newRectangle.Height = location.height; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, location.x);//set left position
                Canvas.SetTop(newRectangle, location.y); //set top position 
                invoker.drawnElements.Add(newRectangle);
                returnelement = newRectangle;
            }
            else if (element.Name == "Ellipse")
            {
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = location.width;
                newEllipse.Height = location.height;
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                invoker.drawnElements.Add(newEllipse);
                returnelement = newEllipse;
            }
            return returnelement;
        }

        //resizing element in group
        public FrameworkElement ResizingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            KeyNumber(element, invoker); //move selected at removed
            //create at new size
            if (element.Name == "Rectangle")
            {
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = location.width; //set width
                newRectangle.Height = location.height; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, location.x);
                Canvas.SetTop(newRectangle, location.y);
                invoker.drawnElements.Add(newRectangle);
                returnelement = newRectangle;
            }
            else if (element.Name == "Ellipse")
            {
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = location.width; //set width
                newEllipse.Height = location.height; //set height 
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                invoker.drawnElements.Add(newEllipse);
                returnelement = newEllipse;
            }
            return returnelement;
        }
        */

        /*
        //display lines for saving.
        public string Display(int depth, Group group, IWriter visitor, Canvas paintSurface)
        {
            string str = "";
            //if (depth <= maxdepth)
            //{
                //Display group.
                //group parts counts.
                int groupcount = group.drawnElements.Count() + group.addedGroups.Count();
                int i = 0;
                while (i < depth)
                {
                    str += "\t";
                    i++;
                }
                str = str + "group " + groupcount + "\n"; //group label

                if (groupcount >= 0)
                {
                    int newdepth = depth + 1;
                    //Recursively display child nodes.   
                    if (group.drawnElements.Count() > 0)
                    {
                        int j = 0;
                        //foreach (FrameworkElement child in group.drawnElements)
                        foreach (IComponent component in group.drawnComponents)
                        {
                            FrameworkElement child = group.drawnElements[j];
                            j++;
                            int k = 0;
                            while (k < newdepth)
                            {
                                str += "\t";
                                k++;
                            }
                            str = str + component.Write(visitor, child, paintSurface);
                        }
                    }
                    if (group.addedGroups.Count() > 0)
                    {
                        //Recursively through groups.
                        foreach (Group subgroup in group.addedGroups)
                        {
                            int subgroupcount = subgroup.drawnElements.Count() + subgroup.addedGroups.Count();
                            if (subgroupcount > 0)
                            {
                                //string gstr = "";
                                //WriteClient client = new WriteClient();
                                string substr = subgroup.Display(newdepth, subgroup, visitor, paintSurface);
                                str = str + substr;
                            }
                        }
                    }
                }
            //}
            return str;
        }
        */



        ////display lines for saving
        //public string Display(int depth, Group group)
        //{
        //    //Display group.
        //    string str = "";
        //    //Add group.
        //    int i = 0;
        //    while (i < depth)
        //    {
        //        str += "\t";
        //    }
        //    int groupcount = group.drawnElements.Count() + group.addedGroups.Count();
        //    str = str + "group " + groupcount + "\n";

        //    //Recursively display child nodes.
        //    depth = depth + 1; //add depth tab
        //    if (group.drawnElements.Count() > 0)
        //    {
        //        foreach (FrameworkElement child in group.drawnElements)
        //        {
        //            if (child is Rectangle)
        //            {
        //                int j = 0;
        //                while (j < depth)
        //                {
        //                    str += "\t";
        //                    j++;
        //                }
        //                str = str + "rectangle " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
        //            }
        //            //else if (child is Ellipse)
        //            else
        //            {
        //                int j = 0;
        //                while (j < depth)
        //                {
        //                    str += "\t";
        //                    j++;
        //                }
        //                str = str + "ellipse " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
        //            }
        //        }
        //    }
        //    if (group.addedGroups.Count() > 0)
        //    {
        //        foreach (Group subgroup in group.addedGroups)
        //        {
        //            subgroup.Display(depth + 1, subgroup);
        //        }
        //    }
        //    return str;
        //}






    }
}