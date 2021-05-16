using System;
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

    //
    //components
    //

    //component interface
    public interface IComponent
    {
        FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location);
        //string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface);
        string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface, Shape shape);

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
        public List<string> ornamentNames = new List<string>();
        public List<string> remvedOrnamentNames = new List<string>();
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

        public override string ToString(double x, double y, double width, double height)
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

        //public override void Add(TextBlock lab, string position, string name)
        public override void Add(string position, string name)
        {
            //this.ornaments.Add(lab);
            this.ornamentPositions.Add(position);
            this.ornamentNames.Add(name);
        }

        public override void Remove()
        {
            //this.ornaments.RemoveAt(this.ornaments.Count() - 1);
            this.ornamentPositions.RemoveAt(this.ornamentPositions.Count() - 1);
            this.ornamentNames.RemoveAt(this.ornamentNames.Count() - 1);
        }

        //Note that calling ConcreteComponent which matches the current class name. 
        //This way we let the visitor know the class of the component it works with.
        public override FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location)
        {
            FrameworkElement madeElement = visitor.VisitConcreteComponentRectangle(this, invoker, selectedelement, paintSurface, location);
            return madeElement;
        }

        //public override string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface)
        public override string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface, Shape shape)
        {
            string str = visitor.WriteOrnament(shape);
            //string str = visitor.WriteRectangleOrnament(this);
            str = str + visitor.WriteRectangle(this, element, paintSurface);
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
        public List<string> ornamentNames = new List<string>();
        public List<string> remvedOrnamentNames = new List<string>();
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

        //public override void Add(TextBlock lab, string position, string name)
        public override void Add(string position, string name)
        {
            //this.ornaments.Add(lab);
            this.ornamentPositions.Add(position);
            this.ornamentNames.Add(name);
        }

        public override void Remove()
        {
            //this.ornaments.RemoveAt(this.ornaments.Count() - 1);
            this.ornamentPositions.RemoveAt(this.ornamentPositions.Count() - 1);
            this.ornamentNames.RemoveAt(this.ornamentNames.Count() - 1);
        }

        public override string ToString(double x, double y, double width, double height)
        {
            string str = "ellipse" + x + " " + y + " " + width + " " + height;
            return str;
        }

        //public override string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface)
        public override string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface, Shape shape)
        {
            //string str = visitor.WriteEllipseOrnament(this);
            string str = visitor.WriteOrnament(shape);
            str = str + visitor.WriteEllipse(this, element, paintSurface);
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

}
