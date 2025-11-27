using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DocumentManagementService
{
    public class DebounceBehavior : Behavior<Button>
    {
        private static readonly TimeSpan DebounceTime = TimeSpan.FromSeconds(10);
        private DateTime _lastClickTime = DateTime.MinValue;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnButtonClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= OnButtonClick;
            base.OnDetaching();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (DateTime.Now - _lastClickTime < DebounceTime)
            {
                e.Handled = true;
                return;
            }

            _lastClickTime = DateTime.Now;
        }
    }
}
