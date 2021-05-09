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
    public class Shape
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public Invoker invoker;
        public Canvas paintSurface;

        public FrameworkElement prevelement; //prev element
        public FrameworkElement nextelement; //next element
        public FrameworkElement selectedElement; //selected element

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

        // Selects the shape
        public void Select(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = e.OriginalSource as FrameworkElement;
            selectedElement.Opacity = 0.6; //fill opacity
            invoker.selectElementsList.Add(selectedElement);
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
        }

        // Deselects the shape
        public void Deselect(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = invoker.selectElementsList.Last();
            selectedElement.Opacity = 1; //fill opacity
            invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
            invoker.unselectElementsList.Add(selectedElement);
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.UnselectGroup(selectedElement, invoker);
        }

        // Reselect the shape
        public void Reselect(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = invoker.unselectElementsList.Last();
            selectedElement.Opacity = 0.6; //fill opacity
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1);
            invoker.selectElementsList.Add(selectedElement);
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
        }



        //
        //repaint
        //
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
        }

        //
        //creation
        //

        //create rectangle
        public void MakeRectangle(Invoker invoker, Canvas paintSurface)
        {
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
            Repaint(invoker, paintSurface); //repaint
        }

        //create ellipse
        public void MakeEllipse(Invoker invoker, Canvas paintSurface)
        {
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
            Repaint(invoker, paintSurface); //repaint
        }

        //
        //undo redo
        //

        //undo create
        public void Remove(Invoker invoker, Canvas paintSurface)
        {
            //remove previous
            prevelement = invoker.drawnElements.Last();
            invoker.removedElements.Add(prevelement);
            invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            Repaint(invoker, paintSurface); //repaint
        }

        //redo create
        public void Add(Invoker invoker, Canvas paintSurface)
        {
            //create next
            nextelement = invoker.removedElements.Last();
            invoker.drawnElements.Add(nextelement);
            invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
            Repaint(invoker, paintSurface); //repaint
        }

        //
        //moving
        //

        //new moving shape
        public void Moving(Invoker invoker, Canvas paintSurface, Location location, FrameworkElement element)
        {
            //FrameworkElement element = invoker.selectElementsList.Last();
            KeyNumber(element, invoker); //move selected at removed
            //create at new location
            MovingElement(element, invoker, paintSurface, location);
            Repaint(invoker, paintSurface); //repaint
        }


        public FrameworkElement MovingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
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

        //move back element
        public void MoveBack(Invoker invoker, Canvas paintSurface)
        {
            //remove next
            prevelement = invoker.drawnElements.Last();
            invoker.removedElements.Add(prevelement);
            invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            //move back moved element
            nextelement = invoker.movedElements.Last();
            invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
            invoker.drawnElements.Add(nextelement);
            Repaint(invoker, paintSurface); //repaint   
        }

        //move back element
        public void MoveAgain(Invoker invoker, Canvas paintSurface)
        {
            //remove previous
            prevelement = invoker.drawnElements.Last();
            nextelement = invoker.removedElements.Last();
            invoker.removedElements.Add(prevelement);
            invoker.movedElements.Add(prevelement);
            invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            //move again moved element
            invoker.drawnElements.Add(nextelement);
            Repaint(invoker, paintSurface); //repaint   
        }

        //
        //resizing
        //

        //resize shape
        public void Resize(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement element)
        {
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
        //saving
        //
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
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp3data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
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
            invoker.movedElements.Clear();
            invoker.selectElementsList.Clear();
            invoker.unselectElementsList.Clear();
            invoker.drawnGroups.Clear();
            invoker.removedGroups.Clear();
            invoker.movedGroups.Clear();
            invoker.selectedGroups.Clear();
            invoker.unselectedGroups.Clear();
            invoker.executer = 0;
            invoker.counter = 0;
            //read file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile saveFile = await storageFolder.GetFileAsync("dp3data.txt");
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
            for (int inc = readText.Count(); inc > 0; inc--)
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
                        OrnamentDecorator ornament = new OrnamentDecorator();
                        FrameworkElement addToElement = invoker.drawnElements[ec];
                        ornament.Draw(addToElement, line[2], line[1], invoker); //element, name, position, invoker
                    }
                    //add to group
                    else if(ge =="group")
                    {
                        OrnamentDecorator ornament = new OrnamentDecorator();
                        FrameworkElement addToElement = null;
                        ornament.Draw(addToElement, line[2], line[1], invoker); //element, name, position, invoker
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

    }

}

