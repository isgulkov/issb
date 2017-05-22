using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace issb
{
    public class ToolboxItem : ContentControl
    {
        private Point? DragStartPoint = null;

        static ToolboxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolboxItem), new FrameworkPropertyMetadata(typeof(ToolboxItem)));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs eventArgs)
        {
            base.OnPreviewMouseDown(eventArgs);
            DragStartPoint = new Point?(eventArgs.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs eventArgs)
        {
            base.OnMouseMove(eventArgs);

            if(eventArgs.LeftButton != MouseButtonState.Pressed) {
                DragStartPoint = null;
            }

            if(DragStartPoint.HasValue) {
                Point position = eventArgs.GetPosition(this);
                if((SystemParameters.MinimumHorizontalDragDistance <= Math.Abs(position.X - DragStartPoint.Value.X)) ||
                    (SystemParameters.MinimumVerticalDragDistance <= Math.Abs(position.Y - DragStartPoint.Value.Y))) {
                    string xamlString = XamlWriter.Save(Content);

                    DataObject dataObject = new DataObject("DESIGNER_ITEM", xamlString);

                    if(dataObject != null) {
                        DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
                    }
                }

                eventArgs.Handled = true;
            }
        }
    }
}
