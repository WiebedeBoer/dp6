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

    //moving visitor client for group
    public class MoveClient
    {
        //The client code can run visitor operations over any set of elements without figuring out their concrete classes. 
        //The accept operation directs a call to the appropriate operation in the visitor object.
        public void Client(List<IComponent> components, List<FrameworkElement> drawnElements, Group selectedgroup, IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            invoker.removedGroups.Add(selectedgroup);
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
    }

    //resizing client for group
    public class ResizeClient
    {
        public void Client(List<IComponent> components, List<FrameworkElement> drawnElements, Group selectedgroup, IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            invoker.removedGroups.Add(selectedgroup);
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
    }

    //
    //components
    //

    //component interface
    public interface IComponent
    {
        FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location);
        string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface);

        string ToString(double x, double y, double width, double height);

        FrameworkElement Execute(double left, double top, double width, double height, FrameworkElement g, bool selected, Invoker invoker);
    }

    //rectangle component singleton
    public class ConcreteComponentRectangle : Strategy
    {
        public double x;
        public double y;
        public double width;
        public double height;
        public static Strategy strategy;

        public FrameworkElement element;

        public List<TextBlock> ornaments = new List<TextBlock>();
        public List<TextBlock> removedOrnaments = new List<TextBlock>();
        public List<string> ornamentPositions = new List<string>();
        public List<string> removedOrnamentPositions = new List<string>();

        //public ConcreteComponentRectangle(double x, double y, double width, double height)
        private ConcreteComponentRectangle()
        //public ConcreteComponentRectangle()
        {
            //this.x = x;
            //this.y = y;
            //this.width = width;
            //this.height = height;
        }

        // Prevents the strategy from instantiating multiple times
        public static Strategy GetInstance()
        {
            if (strategy == null)
            {
                strategy = new ConcreteComponentRectangle();
            }
            return strategy;
        }

        public override String ToString(double x, double y, double width, double height)
        {
            string str = "rectangle" + x + " " + y + " " + width + " " + height;
            return str;
        }

        public override void Element(FrameworkElement element)
        {
            this.element = element;
        }

        public override FrameworkElement GetElement()
        {
            return this.element;
        }

        public override void Add(TextBlock lab, string position)
        {
            this.ornaments.Add(lab);
            this.ornamentPositions.Add(position);
        }

        public override void Remove()
        {
            this.ornaments.RemoveAt(this.ornaments.Count() -1);
            this.ornamentPositions.RemoveAt(this.ornamentPositions.Count() -1);
        }

        //Note that calling ConcreteComponent which matches the current class name. 
        //This way we let the visitor know the class of the component it works with.
        public override FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location)
        {
            FrameworkElement madeElement = visitor.VisitConcreteComponentRectangle(this, invoker, selectedelement, paintSurface, location);
            return madeElement;
        }

        public override string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface)
        {
            string str = visitor.WriteRectangle(this, element, paintSurface);
            return str;
        }

        // Concrete Components may have special methods that don't exist in their base class or interface. 
        //The Visitor is still able to use these methods since it's aware of the component's concrete class.
        public override FrameworkElement Execute(double x, double y, double width, double height, FrameworkElement g, bool selected, Invoker invoker)
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
            g = newRectangle;

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            return g;

        }
    }

    //ellipse component singleton
    public class ConcreteComponentEllipse : Strategy
    {
        public double x;
        public double y;
        public double width;
        public double height;
        public static Strategy strategy;

        public FrameworkElement element { get; set; }

        public List<TextBlock> ornaments = new List<TextBlock>();
        public List<TextBlock> removedOrnaments = new List<TextBlock>();
        public List<string> ornamentPositions = new List<string>();
        public List<string> removedOrnamentPositions = new List<string>();

        //public ConcreteComponentEllipse(double x, double y, double width, double height)
        //public ConcreteComponentEllipse()
        private ConcreteComponentEllipse()
        {
            //this.x = x;
            //this.y = y;
            //this.width = width;
            //this.height = height;
        }

        // Prevents the strategy from instantiating multiple times
        public static Strategy GetInstance()
        {
            if (strategy == null)
            {
                strategy = new ConcreteComponentEllipse();
            }
            return strategy;
        }

        // Same here: ConcreteComponent => ConcreteComponent
        public override FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location)
        {
            FrameworkElement madeElement = visitor.VisitConcreteComponentEllipse(this, invoker, selectedelement, paintSurface, location);
            return madeElement;
        }

        public override void Element(FrameworkElement element)
        {
            this.element = element;
        }

        public override FrameworkElement GetElement()
        {
            return this.element;
        }

        public override void Add(TextBlock lab, string position)
        {
            this.ornaments.Add(lab);
            this.ornamentPositions.Add(position);
        }

        public override void Remove()
        {
            this.ornaments.RemoveAt(this.ornaments.Count() - 1);
            this.ornamentPositions.RemoveAt(this.ornamentPositions.Count() - 1);
        }

        public override String ToString(double x, double y, double width, double height)
        {
            string str = "ellipse" + x + " " + y + " " + width + " " + height;
            return str;
        }

        public override string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface)
        {
            string str = visitor.WriteEllipse(this, element, paintSurface);
            return str;
        }

        public override FrameworkElement Execute(double x, double y, double width, double height, FrameworkElement g, bool selected, Invoker invoker)
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
            g = newEllipse;

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            return g;
        }
    }

    //visitor interface
    public interface IVisitor
    {
        //FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location);

        //FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location);
        FrameworkElement VisitConcreteComponentRectangle(Strategy component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location);

        FrameworkElement VisitConcreteComponentEllipse(Strategy component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location);

    }

    //move visitor
    class ConcreteVisitorMove : IVisitor
    {
        //public FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location)
        public FrameworkElement VisitConcreteComponentRectangle(Strategy component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location)
        {
            KeyNumber(element, invoker); //move selected at removed
            FrameworkElement returnelement = null;
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
            return returnelement;
        }

        public FrameworkElement VisitConcreteComponentEllipse(Strategy component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location)
        //public FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location)
        {
            KeyNumber(element, invoker); //move selected at removed
            FrameworkElement returnelement = null;
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

    }

    //resize visitor
    class ConcreteVisitorResize : IVisitor
    {
        public FrameworkElement VisitConcreteComponentRectangle(Strategy component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location)
        //public FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location)
        {
            KeyNumber(element, invoker); //move selected at removed
            FrameworkElement returnelement = null;
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
            return returnelement;
        }

        public FrameworkElement VisitConcreteComponentEllipse(Strategy component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location)
        //public FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location)
        {
            KeyNumber(element, invoker); //move selected at removed
            FrameworkElement returnelement = null;
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

    }

    //write to file client class
    public class WriteClient
    {
        public async void Client(Canvas paintSurface, Invoker invoker, IWriter visitor)
        {
            string lines = ""; //lines to file
            int i = 0;
            //ungrouped and drawn
            foreach (FrameworkElement child in paintSurface.Children)
            {
                int elmcheck = CheckInGroup(invoker, child); //see if already in group
                if (elmcheck == 0)
                {
                    if (child is Rectangle || child is Ellipse)
                    {
                        IComponent component = invoker.drawnComponents[i];
                        //IWriter visitor = new ConcreteVisitorWrite();
                        string str = component.Write(visitor, child, paintSurface);
                        lines += str;
                    }
                }
                i++;
            }
            //grouped and drawn
            foreach (Group group in invoker.drawnGroups)
            {
                string slines = "";
                string gstr = Display(0, group.depth, group, visitor, paintSurface);
                lines += gstr;
            }
            //create and write to file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp6data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, lines);
        }

        //display lines for saving.
        public string Display(int depth, int maxdepth, Group group, IWriter visitor, Canvas paintSurface)
        {
            string str = "";
            if (depth < maxdepth)
            {
                //Display group.
                //group parts counts.
                int groupcount = group.drawnElements.Count() + group.addedGroups.Count();
                
                int i = 0;
                
                //depth++;
                while (i < depth)
                {
                    str += "\t";
                }
                str = str + "group " + groupcount + "\n"; //group label
                depth = depth + 1; //add depth tab

                if (groupcount ==0)
                {
                    return str;
                }
                else { 
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
                            while (k < depth)
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
                                string substr = Display(depth + 1, maxdepth, subgroup, visitor, paintSurface);
                                str = str + substr;
                            }
                        }
                    }
                    return str;
                }
                
                
                return str;
                
            }
            else
            {
                return str;
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
                            return counter;
                        }
                    }
                }
                if (group.addedGroups.Count() > 0 && counter == 0)
                {
                    foreach (Group subgroup in group.addedGroups)
                    {
                        counter = CheckInSubGroup(subgroup, invoker, element);
                        if (counter > 0)
                        {
                            return counter;
                        }
                    }
                }
            }
            return counter;
        }

        //check if element in sub group
        public int CheckInSubGroup(Group group, Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            if (group.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement groupelement in group.drawnElements)
                {
                    if (groupelement.AccessKey == element.AccessKey)
                    {
                        counter++;
                        return counter;
                    }
                }
            }
            if (group.addedGroups.Count() > 0 && counter == 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    counter = CheckInSubGroup(subgroup, invoker, element);
                    if (counter > 0)
                    {
                        return counter;
                    }
                }
            }
            return counter;
        }

    }

    //interface for writing to file
    public interface IWriter
    {
        string WriteRectangle(ConcreteComponentRectangle component, FrameworkElement element, Canvas paintSurface);
        string WriteEllipse(ConcreteComponentEllipse component, FrameworkElement element, Canvas paintSurface);
    }

    //concrete writer class
    public class ConcreteVisitorWrite : IWriter
    {

        public string WriteRectangle(ConcreteComponentRectangle component, FrameworkElement element, Canvas paintSurface)
        {
            double top = (double)element.GetValue(Canvas.TopProperty);
            double left = (double)element.GetValue(Canvas.LeftProperty);
            string str = "rectangle " + left + " " + top + " " + element.Width + " " + element.Height + "\n";
            //lines += str;
            return str;
        }

        public string WriteEllipse(ConcreteComponentEllipse component, FrameworkElement element, Canvas paintSurface)
        {
            double top = (double)element.GetValue(Canvas.TopProperty);
            double left = (double)element.GetValue(Canvas.LeftProperty);
            string str = "ellipse " + left + " " + top + " " + element.Width + " " + element.Height + "\n";
            //lines += str;
            return str;
        }

    }



}
