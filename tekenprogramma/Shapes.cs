﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Windows.Storage;
using Windows.ApplicationModel.Activation;

namespace tekenprogramma
{







    //shape class
    public class Shape : IDecoratorShape
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public Invoker invoker;
        public Canvas paintSurface;

        public FrameworkElement madeelement; //made element
        public string type; //made type

        public FrameworkElement prevelement; //prev element
        public FrameworkElement nextelement; //next element
        public FrameworkElement selectedElement; //selected element

        public List<FrameworkElement> drawnElements = new List<FrameworkElement>();
        public List<FrameworkElement> removedElements = new List<FrameworkElement>();
        public List<FrameworkElement> movedElements = new List<FrameworkElement>();
        public List<FrameworkElement> unmovedElements = new List<FrameworkElement>();
        public List<FrameworkElement> selectElementsList = new List<FrameworkElement>();
        public List<FrameworkElement> unselectElementsList = new List<FrameworkElement>();

        public IComponent prevcomponent;
        public IComponent nextcomponent;

        public List<TextBlock> ornaments = new List<TextBlock>();
        public List<TextBlock> removedOrnaments = new List<TextBlock>();
        public List<string> ornamentNames = new List<string>();
        public List<string> removedOrnamentNames = new List<string>();
        public List<string> removedOrnamentKeys = new List<string>();
        public List<string> undoOrnamentNames = new List<string>();
        public List<string> ornamentPositions = new List<string>();
        public List<string> removedOrnamentPositions = new List<string>();
        public List<string> OrnamentKeys = new List<string>();
        //for undo redo ornaments
        public List<string> ornamentKeyNames = new List<string>();
        public List<string> undoKeyNames = new List<string>();

        //file IO
        public string fileText { get; set; }

        //shape
        public Shape(double x, double y, double width, double height) 
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Shape Execute()
        {
            return this;
        }

        public Group Fetch()
        {
            return null;
        }

        public void Draw() { }
        public void Add(string position, string name) { }

        // Selects the shape
        public void Select(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = e.OriginalSource as FrameworkElement;
            selectedElement.Opacity = 0.6; //fill opacity
            invoker.selectElements.Add(selectedElement);
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
        }

        // Deselects the shape
        public void Deselect(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = invoker.selectElements.Last(); //err
            selectedElement.Opacity = 1; //fill opacity
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1);
            invoker.unselectElements.Add(selectedElement);
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.UnselectGroup(selectedElement, invoker);
        }

        // Reselect the shape
        public void Reselect(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = invoker.unselectElements.Last();
            selectedElement.Opacity = 0.6; //fill opacity
            invoker.unselectElements.RemoveAt(invoker.unselectElements.Count() - 1);
            invoker.selectElements.Add(selectedElement);
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
        }



        //
        //repaint
        //

        //repaint
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            int i = 0;
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
                Shape shape = invoker.drawnShapes[i];
                foreach (string ornament in shape.ornamentNames)
                {
                    OrnamentDecorator deco = new OrnamentDecorator(shape);
                    deco.Draw(drawelement, ornament, shape.ornamentPositions[i], invoker, false);
                    i++;
                }
            }
            FrameworkElement groupelement = null;
            foreach (Group group in invoker.drawnGroups)
            {
                foreach (string ornament in group.ornamentNames)
                {
                    OrnamentDecorator deco = new OrnamentDecorator(group);
                    deco.Draw(groupelement, ornament, group.ornamentPositions[i], invoker, false);
                }
            }
            //foreach (FrameworkElement drawornament in invoker.drawnOrnaments)
            //{
            //    paintSurface.Children.Add(drawornament); //add
            //}
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

        //prepare undo
        public void PrepareUndo(Invoker invoker)
        {
            List<FrameworkElement> PrepareUndo = new List<FrameworkElement>();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                PrepareUndo.Add(drawelement); //add
            }
            invoker.undoElementsList.Add(PrepareUndo);
        }

        //
        //creation
        //

        //create rectangle
        public void MakeRectangle(Invoker invoker, Canvas paintSurface)
        {
            //prepare undo
            PrepareUndo(invoker);
            //create
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
            invoker.drawnElements.Add(newRectangle);
            

            Strategy component = ConcreteComponentRectangle.GetInstance();
            component.Element(newRectangle);
            invoker.drawnComponents.Add(component);

            this.madeelement = newRectangle;
            invoker.drawnShapes.Add(this);

            Repaint(invoker, paintSurface); //repaint
        }

        //create ellipse
        public void MakeEllipse(Invoker invoker, Canvas paintSurface)
        {
            //prepare undo
            PrepareUndo(invoker);
            //create
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
            invoker.drawnElements.Add(newEllipse);
            

            Strategy component = ConcreteComponentEllipse.GetInstance();
            component.Element(newEllipse);
            invoker.drawnComponents.Add(component);

            this.madeelement = newEllipse;
            invoker.drawnShapes.Add(this);

            Repaint(invoker, paintSurface); //repaint
        }

        //
        //undo redo
        //

        //undo create
        public void Remove(Invoker invoker, Canvas paintSurface)
        {
            /*
            //remove previous
            prevelement = invoker.drawnElements.Last();
            invoker.removedElements.Add(prevelement);
            invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            Repaint(invoker, paintSurface); //repaint
            */
            List<FrameworkElement> lastUndo = invoker.undoElementsList.Last();
            invoker.redoElementsList.Add(lastUndo);
            invoker.undoElementsList.RemoveAt(invoker.undoElementsList.Count() - 1);
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in lastUndo)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            foreach (TextBlock drawornament in invoker.drawnOrnaments)
            {
                paintSurface.Children.Add(drawornament); //add
            }
        }

        //redo create
        public void Add(Invoker invoker, Canvas paintSurface)
        {
            /*
            //create next
            nextelement = invoker.removedElements.Last();
            invoker.drawnElements.Add(nextelement);
            invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
            Repaint(invoker, paintSurface); //repaint
            */
            List<FrameworkElement> lastRedo = invoker.redoElementsList.Last();
            invoker.undoElementsList.Add(lastRedo);
            invoker.redoElementsList.RemoveAt(invoker.redoElementsList.Count() - 1);
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in lastRedo)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            foreach (TextBlock drawornament in invoker.drawnOrnaments)
            {
                paintSurface.Children.Add(drawornament); //add
            }
        }

        //
        //moving
        //

        //new moving shape
        public void Moving(Invoker invoker, Canvas paintSurface, Location location, FrameworkElement element)
        {
            //prepare undo
            PrepareUndo(invoker);
            //FrameworkElement element = invoker.selectElementsList.Last();
            KeyNumber(element, invoker); //move selected at removed
            //create at new location
            MovingElement(element, invoker, paintSurface, location);
            Repaint(invoker, paintSurface); //repaint
        }


        public FrameworkElement MovingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {

            //moving
            FrameworkElement returnelement = null;
            //add selected to unselect
            invoker.unselectElements.Add(element); //2b+
            //remove from selected
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //add to moved
            invoker.movedElements.Add(element); //3a+
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
                //add new to drawn
                invoker.drawnElements.Add(newRectangle); //1+
                //add undo
                //invoker.unmovedElements.Add(newRectangle); //3b+
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
                //add new to drawn
                invoker.drawnElements.Add(newEllipse); //1+
                //add undo
                //invoker.unmovedElements.Add(newEllipse); //3b+
                returnelement = newEllipse;
            }
            this.movedElements.Add(returnelement);

            //foreach (Shape shape in invoker.drawnShapes)
            //{
            //    if (shape.madeelement.AccessKey ==element.AccessKey)
            //    {
            //        shape.madeelement = returnelement;               
            //    }
            //}
            return returnelement;
        }

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
            //invoker.removedElements.Add(element);
            //invoker.movedElements.Add(element);
        }

        //move back element
        public void MoveBack(Invoker invoker, Canvas paintSurface)
        {
            List<FrameworkElement> lastUndo = invoker.undoElementsList.Last();
            invoker.redoElementsList.Add(lastUndo);
            invoker.undoElementsList.RemoveAt(invoker.undoElementsList.Count() - 1);
            //repaint
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in lastUndo)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            foreach (TextBlock drawornament in invoker.drawnOrnaments)
            {
                paintSurface.Children.Add(drawornament); //add
            }

            //shuffle unselected
            //prevelement = invoker.movedElements.Last();
            prevelement = invoker.unselectElements.Last();
            invoker.unselectElements.RemoveAt(invoker.unselectElements.Count() - 1); //2b-
            invoker.selectElements.Add(prevelement); //2a+
            /*
            //shuffle moved
            nextelement = invoker.unmovedElements.Last();
            invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1); //3a-
            invoker.unmovedElements.RemoveAt(invoker.unmovedElements.Count() - 1); //3b-
            //add redo
            invoker.undoElements.Add(nextelement); //4a+
            invoker.redoElements.Add(prevelement); //4b+
            //remove and add to drawn
            KeyNumber(nextelement, invoker); //1-
            invoker.drawnElements.Add(prevelement); //1+
            //repaint surface
            Repaint(invoker, paintSurface);
            */
        }

        //move back element
        public void MoveAgain(Invoker invoker, Canvas paintSurface)
        {

            List<FrameworkElement> lastRedo = invoker.redoElementsList.Last();
            invoker.undoElementsList.Add(lastRedo);
            invoker.redoElementsList.RemoveAt(invoker.redoElementsList.Count() - 1);
            //repaint
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in lastRedo)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            foreach (TextBlock drawornament in invoker.drawnOrnaments)
            {
                paintSurface.Children.Add(drawornament); //add
            }

            //shuffle selected
            //nextelement = invoker.undoElements.Last();
            nextelement = invoker.selectElements.Last();
            invoker.unselectElements.Add(nextelement); //2b+
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            /*
            //shuffle moved
            prevelement = invoker.redoElements.Last();
            invoker.movedElements.Add(prevelement); //3a+
            invoker.unmovedElements.Add(nextelement); //3b+
            //undo redo
            invoker.undoElements.RemoveAt(invoker.undoElements.Count() - 1); ; //4a-
            invoker.redoElements.RemoveAt(invoker.redoElements.Count() - 1); ; //4b-
            //remove and add to drawn
            KeyNumber(prevelement, invoker); //1-
            invoker.drawnElements.Add(nextelement); //1+
            //repaint surface
            Repaint(invoker, paintSurface);
            */
        }

        //
        //resizing
        //

        //resize shape
        public void Resize(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement element)
        {
            //prepare undo
            PrepareUndo(invoker);
            //resizing
            //FrameworkElement element = invoker.selectElementsList.Last();
            KeyNumber(element, invoker); //move selected at removed

            //calculate size
            double ex = e.GetCurrentPoint(paintSurface).Position.X;
            double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            double lw = Convert.ToDouble(element.ActualOffset.X); //set width
            double lh = Convert.ToDouble(element.ActualOffset.Y); //set height
            double w = ReturnSmallest(ex, lw);
            double h = ReturnSmallest(ey, lh);

            Location location = new Location();
            location.x = Convert.ToDouble(element.ActualOffset.X);
            location.y = Convert.ToDouble(element.ActualOffset.Y);
            location.width = w;
            location.height = h;

            ResizingElement(invoker, e, paintSurface, location, element);
            Repaint(invoker, paintSurface); //repaint
        }

        public FrameworkElement ResizingElement(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, Location location, FrameworkElement element)
        {
            FrameworkElement returnelement = null;
            //add selected to unselect
            invoker.unselectElements.Add(element); //2b+
            //remove from selected
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //add to moved
            invoker.movedElements.Add(element); //3a+
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
                //add new to drawn
                invoker.drawnElements.Add(newRectangle); //1+
                //add undo
                invoker.unmovedElements.Add(newRectangle); //3b+
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
                //add new to drawn
                invoker.drawnElements.Add(newEllipse); //1+
                //add undo
                invoker.unmovedElements.Add(newEllipse); //3b+
                returnelement = newEllipse;
            }
            this.movedElements.Add(returnelement);

            //foreach (Shape shape in invoker.drawnShapes)
            //{
            //    if (shape.madeelement.AccessKey == element.AccessKey)
            //    {
            //        shape.madeelement = returnelement;
            //    }
            //}
            return returnelement;
        }

        //give smallest
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
            }
            return counter;
        }       


        //
        //loading
        //
        public async void Loading(Canvas paintSurface, Invoker invoker)
        {
            //clear previous canvas
            paintSurface.Children.Clear();
            //clear invoker
            invoker.drawnElements.Clear();
            invoker.removedElements.Clear();
            invoker.unmovedElements.Clear();
            invoker.movedElements.Clear();
            invoker.selectElements.Clear();
            invoker.unselectElements.Clear();
            invoker.undoElements.Clear();
            invoker.redoElements.Clear();
            invoker.drawnGroups.Clear();
            invoker.removedGroups.Clear();
            invoker.movedGroups.Clear();
            invoker.unmovedGroups.Clear();
            invoker.selectedGroups.Clear();
            invoker.unselectedGroups.Clear();
            invoker.undoGroups.Clear();
            invoker.redoGroups.Clear();
            //clear components
            invoker.drawnComponents.Clear();
            invoker.removedComponents.Clear();
            invoker.movedComponents.Clear();
            //ornaments
            invoker.drawnOrnaments.Clear();
            invoker.removedOrnaments.Clear();
            //invoker.selectComponentsList.Clear();
            //invoker.unselectComponentsList.Clear();
            invoker.executer = 0;
            invoker.counter = 0;
            //read file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile saveFile = await storageFolder.GetFileAsync("dp6data.txt");
            string text = await Windows.Storage.FileIO.ReadTextAsync(saveFile);
            //load shapes
            string[] readText = Regex.Split(text, "\\n+");
            int i = 0;
            //make groups and shapes
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    invoker.executer++;
                    i++;
                    string notabs = s.Replace("\t", "");
                    string[] line = Regex.Split(notabs, "\\s+");
                    //remake shapes
                    if (line[0] == "ellipse")
                    {
                        Shape shape = new Shape(Convert.ToDouble(line[1]), Convert.ToDouble(line[2]), Convert.ToDouble(line[3]), Convert.ToDouble(line[4]));
                        ICommand place = new MakeEllipses(shape, invoker, paintSurface);
                        invoker.Execute(place);
                    }
                    else if (line[0] == "rectangle")
                    {
                        Shape shape = new Shape(Convert.ToDouble(line[1]), Convert.ToDouble(line[2]), Convert.ToDouble(line[3]), Convert.ToDouble(line[4]));
                        ICommand place = new MakeRectangles(shape, invoker, paintSurface);
                        invoker.Execute(place);
                    }
                    //remake groups
                    else if (line[0] == "group")
                    {
                        FrameworkElement selectedElement = null;
                        Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
                        ICommand place = new MakeGroup(group, paintSurface, invoker);
                        invoker.Execute(place);
                    }
                }
            }
            //re add ornaments to elements
            int ec = invoker.drawnElements.Count();
            int gc = invoker.drawnGroups.Count();
            string ge = "";
            //foreach (string s in readText)
            for (int inc = readText.Count() - 1; inc >=0; inc--)
            {
                string s = readText[inc];
                string notabs = s.Replace("\t", "");
                string[] line = Regex.Split(notabs, "\\s+");
                //remake shapes
                if (line[0] == "ellipse")
                {
                    ec--;
                    ge = "element";
                }
                else if (line[0] == "rectangle")
                {
                    ec--;
                    ge = "element";
                }
                else if (line[0] == "group")
                {
                    gc--;
                    ge = "group";
                    Group selectedgroup = invoker.drawnGroups[gc];
                    invoker.selectedGroups.Add(selectedgroup);
                }
                else if (line[0] == "ornament")
                {
                    //add to element
                    if (ge =="element")
                    {                     
                        FrameworkElement addToElement = invoker.drawnElements[ec];
                        //OrnamentDecorator ornament = new OrnamentDecorator(addToElement.ActualOffset.X, addToElement.ActualOffset.Y, addToElement.Width, addToElement.Height);
                        Shape shape = invoker.drawnShapes[ec];
                        OrnamentDecorator ornament = new OrnamentDecorator(shape);
                        ICommand place = new MakeOrnament(ornament, paintSurface, invoker, addToElement, line[2], line[1]);
                        invoker.Execute(place);

                    }
                    //add to group
                    else if(ge =="group")
                    {
                        //FrameworkElement addToElement = null;
                        FrameworkElement addToElement = invoker.drawnElements[ec];
                        //OrnamentDecorator ornament = new OrnamentDecorator(addToElement.ActualOffset.X, addToElement.ActualOffset.Y, addToElement.Width, addToElement.Height);
                        Group group = invoker.drawnGroups[gc];
                        OrnamentDecorator ornament = new OrnamentDecorator(group);
                        ICommand place = new MakeOrnament(ornament, paintSurface, invoker, addToElement, line[2], line[1]);
                        invoker.Execute(place);
                    }

                }
            }
            //clear selected groups afterwards
            int rme = 0;
            foreach (Group group in invoker.drawnGroups)
            {
                invoker.selectedGroups.RemoveAt(rme);
                rme++;
            }

            int j = 0; //line increment
            int g = 0;//group increment
            //re add elements to groups
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    string notabs = s.Replace("\t", "");
                    string[] line = Regex.Split(notabs, "\\s+");
                    if (line[0] == "group")
                    {
                        GetGroupElements(readText, j, Convert.ToInt32(line[1]), g, invoker);
                        g++;
                    }
                    j++;
                }
            }
            int maingroup = 0; //main group increment
            int k = 0; //line increment
            //remake subgroups and add elements
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    string[] line = Regex.Split(s, "\\s+");
                    int tabcount = s.Length - s.Replace("/", "").Length;

                    //if (s[0] != '\t')
                    if (tabcount < 0)
                    {
                        if (line[0] == "group")
                        {
                            GetSubGroups(readText, maingroup, 0, k, k + Convert.ToInt32(line[1]), invoker);
                        }
                    }
                    maingroup++;
                }
                k++;
            }
            
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

        //re attach element to group
        public void GetGroupElements(string[] readText, int start, int stop, int group, Invoker invoker)
        {
            Group attachgroup = invoker.drawnGroups[group];
            for (int i = start; i < stop; i++)
            {
                FrameworkElement elm = invoker.drawnElements[i];
                attachgroup.drawnElements.Add(elm);
            }

        }

        //re attach subgroups to group
        public void GetSubGroups(string[] readText, int group, int depth, int start, int stop, Invoker invoker)
        {
            Group maingroup = invoker.drawnGroups[group];
            while (start < stop)
            {
                string s = readText[start];
                string notabs = s.Replace("\t", "");
                string tab = "\t";
                int tablength = tab.Length;
                int notablength = notabs.Length;
                int slength = s.Length;
                int subdepth = (slength - notablength) / tablength;

                if (subdepth == (depth + 1))
                {
                    string[] line = Regex.Split(notabs, "\\s+");
                    if (line[0] == "group")
                    {
                        group++;
                        Group subgroup = invoker.drawnGroups[group];
                        maingroup.addedGroups.Add(subgroup);
                        invoker.drawnGroups.RemoveAt(group);

                        GetSubGroups(readText, group, depth + 1, start, start + Convert.ToInt32(line[1]), invoker);
                    }
                    start++;
                }
            }
        }


        //public List<FrameworkElement> drawnElements = new List<FrameworkElement>();
        //public List<FrameworkElement> removedElements = new List<FrameworkElement>();
        //public List<FrameworkElement> movedElements = new List<FrameworkElement>();
        //public List<FrameworkElement> selectElementsList = new List<FrameworkElement>();
        //public List<FrameworkElement> unselectElementsList = new List<FrameworkElement>();


        /*
        //get ornaments of elements
        public void GetElementsOrnaments(Canvas paintSurface, Invoker invoker)
        {

        }

        //get ornament of groups
        public void GetGroupsOrnaments(Canvas paintSurface, Invoker invoker)
        {

        }
        */

        //
        //saving
        //

        /*
        public async void Saving(Canvas paintSurface, Invoker invoker)
        {

            try
            {
                string lines = "";
                //ungrouped and drawn
                foreach (FrameworkElement child in paintSurface.Children)
                {
                    int elmcheck = CheckInGroup(invoker, child); //see if already in group
                    if (elmcheck == 0)
                    {
                        if (child is Rectangle)
                        {
                            //ornament
                            //if (child.ornament.Text.Length > 5 && child.ornamentPosition.Length > 1)
                            //{
                            //    string ostr = "ornament " + child.ornamentPosition + " " + child.ornament.Text;
                            //}
                            //element
                            double top = (double)child.GetValue(Canvas.TopProperty);
                            double left = (double)child.GetValue(Canvas.LeftProperty);
                            string str = "rectangle " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                            lines += str;
                        }
                        else if (child is Ellipse)
                        {
                            //ornament
                            //if (child.ornament.Text.Length > 5 && child.ornamentPosition.Length > 1)
                            //{
                            //    string ostr = "ornament " + child.ornamentPosition + " " + child.ornament.Text;
                            //}
                            //element
                            double top = (double)child.GetValue(Canvas.TopProperty);
                            double left = (double)child.GetValue(Canvas.LeftProperty);
                            string str = "ellipse " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                            lines += str;
                        }
                    }
                }
                //grouped and drawn
                foreach (Group group in invoker.drawnGroups)
                {
                    //ornament
                    if (group.ornament.Text.Length >5 && group.ornamentPosition.Length >1)
                    {
                        string ostr = "ornament " + group.ornamentPosition + " " + group.ornament.Text;
                    }
                    //group
                    string gstr = group.Display(0, group);
                    lines += gstr;
                }
                //create and write to file
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp6data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, lines);
            }
            //file errors
            catch (System.IO.FileNotFoundException)
            {
                fileText = "File not found.";
            }
            catch (System.IO.FileLoadException)
            {
                fileText = "File Failed to load.";
            }
            catch (System.IO.IOException e)
            {
                fileText = "File IO error " + e;
            }
            catch (Exception err)
            {
                fileText = err.Message;
            }

        }
        */



    }

}

