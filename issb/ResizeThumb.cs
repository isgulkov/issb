﻿using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace issb
{
    /// <summary>
    /// Элемент управления, предназначенный для изменения размера элемента раскадровки
    /// </summary>
    class ResizeThumb : Thumb
    {
        /// <summary>
        /// Пермещает элемент раскадровки при перетаскивании данного элемента управления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs eventArgs)
        {
            Control item = DataContext as Control;

            if(item != null) {
                double dVertical, dHorizontal;

                switch(VerticalAlignment) {
                    case System.Windows.VerticalAlignment.Top:
                        dVertical = Math.Min(
                            eventArgs.VerticalChange,
                            item.ActualHeight - item.MinHeight
                            );
                        Canvas.SetTop(item, Canvas.GetTop(item) + dVertical);
                        item.Height -= dVertical;
                        break;
                    case System.Windows.VerticalAlignment.Bottom:
                        dVertical = Math.Min(
                            -eventArgs.VerticalChange,
                            item.ActualHeight - item.MinHeight
                            );
                        item.Height -= dVertical;
                        break;
                    default:
                        break;
                }

                switch(HorizontalAlignment) {
                    case System.Windows.HorizontalAlignment.Left:
                        dHorizontal = Math.Min(
                            eventArgs.HorizontalChange,
                            item.ActualWidth - item.MinWidth
                            );
                        Canvas.SetLeft(item, Canvas.GetLeft(item) + dHorizontal);
                        item.Width -= dHorizontal;
                        break;
                    case System.Windows.HorizontalAlignment.Right:
                        dHorizontal = Math.Min(
                            -eventArgs.HorizontalChange,
                            item.ActualWidth - item.MinWidth
                            );
                        item.Width -= dHorizontal;
                        break;
                    default:
                        break;
                }
            }

            eventArgs.Handled = true;
        }

        public ResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
        }
    }
}
