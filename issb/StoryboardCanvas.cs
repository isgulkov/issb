using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

namespace issb
{
    public class StoryboardCanvas : Canvas
    {
        private Point? DragStartPoint = null;

        public IEnumerable<StoryboardItem> SelectedItems
        {
            get
            {
                return Children.OfType<StoryboardItem>().Where(x => x.IsSelected);
            }
        }

        public void DeselectAll()
        {
            foreach(StoryboardItem selectedItem in SelectedItems) {
                selectedItem.IsSelected = false;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs eventArgs)
        {
            base.OnMouseDown(eventArgs);

            if(eventArgs.Source == this) {
                DragStartPoint = new Point?(eventArgs.GetPosition(this));
                DeselectAll();
                eventArgs.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs eventArgs)
        {
            base.OnMouseMove(eventArgs);

            if(eventArgs.LeftButton != MouseButtonState.Pressed) {
                DragStartPoint = null;
            }

            if(DragStartPoint.HasValue) {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if(adornerLayer != null) {
                    MultipleSelectionAdorner adorner = new MultipleSelectionAdorner(this, DragStartPoint);
                    if(adorner != null) {
                        adornerLayer.Add(adorner);
                    }
                }

                eventArgs.Handled = true;
            }
        }

        protected override void OnDrop(DragEventArgs eventArgs)
        {
            base.OnDrop(eventArgs);

            if(Children.Count == 3) {
                string a = XamlWriter.Save(Children[0]);
                string b = XamlWriter.Save(Children[1]);
                string c = XamlWriter.Save(Children[2]);
            }

            string xamlString = eventArgs.Data.GetData("STORYBOARD_ITEM") as string;

            if(!String.IsNullOrEmpty(xamlString)) {
                FrameworkElement content = XamlReader.Load(XmlReader.Create(new StringReader(xamlString))) as FrameworkElement;

                if(content != null) {
                    StoryboardItem newItem = new StoryboardItem();
                    newItem.Content = content;

                    Point position = eventArgs.GetPosition(this);

                    if(content.MinHeight != 0 && content.MinWidth != 0) {
                        newItem.Width = content.MinWidth * 2; ;
                        newItem.Height = content.MinHeight * 2;
                    }
                    else {
                        newItem.Width = 65;
                        newItem.Height = 65;
                    }

                    SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                    SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));

                    Children.Add(newItem);

                    DeselectAll();
                    newItem.IsSelected = true;
                }

                eventArgs.Handled = true;
            }
        }
    }
}
