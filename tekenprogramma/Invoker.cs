﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace tekenprogramma
{
    //class invoker
    public class Invoker
    {
        public List<ICommand> actionsList = new List<ICommand>();
        public List<ICommand> redoList = new List<ICommand>();

        //state 0
        public List<FrameworkElement> removedElements = new List<FrameworkElement>(); //0
        //state 1
        public List<FrameworkElement> drawnElements = new List<FrameworkElement>(); //1
        //state 2
        public List<FrameworkElement> selectElements = new List<FrameworkElement>(); //2a
        public List<FrameworkElement> unselectElements = new List<FrameworkElement>(); //2b
        //state 3
        public List<FrameworkElement> movedElements = new List<FrameworkElement>(); //3a
        public List<FrameworkElement> unmovedElements = new List<FrameworkElement>(); //3b
        //state 4
        public List<FrameworkElement> undoElements = new List<FrameworkElement>(); //4a
        public List<FrameworkElement> redoElements = new List<FrameworkElement>(); //4b

        //shapes
        public List<Shape> drawnShapes = new List<Shape>();
        public List<Shape> removedShapes = new List<Shape>();
        public List<Shape> movedShapes = new List<Shape>();

        public List<Shape> selectedShapes = new List<Shape>(); //2a
        public List<Shape> unselectedShapes = new List<Shape>(); //2b

        //groups
        public List<Group> removedGroups = new List<Group>(); //0
        public List<Group> drawnGroups = new List<Group>(); //1       

        public List<Group> selectedGroups = new List<Group>(); //2a
        public List<Group> unselectedGroups = new List<Group>(); //2b

        public List<Group> movedGroups = new List<Group>(); //3a
        public List<Group> unmovedGroups = new List<Group>(); //3b

        public List<Group> undoGroups = new List<Group>(); //4a
        public List<Group> redoGroups = new List<Group>(); //4b

        //components
        public List<IComponent> drawnComponents = new List<IComponent>();
        public List<IComponent> removedComponents = new List<IComponent>();
        public List<IComponent> movedComponents = new List<IComponent>();

        public int counter = 0;
        public int executer = 0;

        public List<TextBlock> drawnOrnaments = new List<TextBlock>();
        public List<TextBlock> removedOrnaments = new List<TextBlock>();
        public List<TextBlock> undoOrnaments = new List<TextBlock>();
        public List<TextBlock> redoOrnaments = new List<TextBlock>();

        public Invoker()
        {
            this.actionsList = new List<ICommand>();
            this.redoList = new List<ICommand>();
        }

        //execute
        public void Execute(ICommand cmd)
        {
            actionsList.Add(cmd);
            redoList.Clear();
            cmd.Execute();
            counter++;
            executer++;
        }

        //undo
        public void Undo()
        {
            if (actionsList.Count >= 1)
            {
                ICommand cmd = actionsList.Last();
                actionsList.RemoveAt(actionsList.Count - 1);
                redoList.Add(cmd);
                cmd.Undo();
                counter--;
            }
            //Repaint();
        }

        //redo
        public void Redo()
        {
            if (redoList.Count >= 1)
            {
                ICommand cmd = redoList.Last();
                actionsList.Add(cmd);
                redoList.RemoveAt(redoList.Count - 1);
                cmd.Redo();
                counter++;
            }
            //Repaint();
        }

        //repaint
        public void Repaint()
        {
            //repaint actions
            //foreach (ICommand icmd in actionsList)
            foreach (ICommand icmd in this.actionsList)
            {
                icmd.Execute();
            }
        }

        //clear
        public void Clear()
        {
            actionsList.Clear();
        }

    }

    //public List<IComponent> selectComponentsList = new List<IComponent>();
    //public List<IComponent> unselectComponentsList = new List<IComponent>();


    //public List<Shape> selectShapes = new List<Shape>();
    //public List<Shape> unselectShapes = new List<Shape>();

    //public List<FrameworkElement> groupedElementsList = new List<FrameworkElement>();
    //public List<FrameworkElement> ungroupedElementsList = new List<FrameworkElement>();
    //public List<Canvas> canvases = new List<Canvas>();
    //public List<Canvas> removedcanvases = new List<Canvas>();
    //public List<Canvas> selectedCanvases = new List<Canvas>();
    //public List<Canvas> unselectedCanvases = new List<Canvas>();
}
