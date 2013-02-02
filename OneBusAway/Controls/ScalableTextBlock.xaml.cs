﻿using OneBusAway.Triggers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneBusAway.Controls
{
    /// <summary>
    /// Defines an object that resizes text based on the width of the app.
    /// </summary>
    public sealed partial class ScalableTextBlock : UserControl
    {
        public static readonly DependencyProperty LargeFontSizeProperty = DependencyProperty.Register("LargeFontSize", 
            typeof(int), 
            typeof(ScalableTextBlock), 
            new PropertyMetadata(14));
        
        public static readonly DependencyProperty NormalFontSizeProperty = DependencyProperty.Register("NormalFontSize", 
            typeof(int), 
            typeof(ScalableTextBlock), 
            new PropertyMetadata(12));
        
        public static readonly DependencyProperty SnappedFontSizeProperty = DependencyProperty.Register("SnappedFontSize", 
            typeof(int), 
            typeof(ScalableTextBlock), 
            new PropertyMetadata(10));
        
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", 
            typeof(string), 
            typeof(ScalableTextBlock), 
            new PropertyMetadata(null));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
            typeof(ICommand),
            typeof(ScalableTextBlock),
            new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter",
            typeof(object),
            typeof(ScalableTextBlock),
            new PropertyMetadata(null));

        public static readonly DependencyProperty VerticalTextAlignmentProperty = DependencyProperty.Register("VerticalTextAlignment",
            typeof(VerticalAlignment),
            typeof(ScalableTextBlock),
            new PropertyMetadata(VerticalAlignment.Top));

        public static readonly DependencyProperty HorizontalTextAlignmentProperty = DependencyProperty.Register("HorizontalTextAlignment",
            typeof(HorizontalAlignment),
            typeof(ScalableTextBlock),
            new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Navigation controller proxy.
        /// </summary>
        private NavigationControllerProxy proxy;

        public ScalableTextBlock()
        {
            this.InitializeComponent();
            
            this.proxy = new NavigationControllerProxy();
            this.proxy.PropertyChanged += OnProxyPropertyChanged;

        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public int SnappedFontSize
        {
            get { return (int)GetValue(SnappedFontSizeProperty); }
            set { SetValue(SnappedFontSizeProperty, value); }
        }

        public int NormalFontSize
        {
            get { return (int)GetValue(NormalFontSizeProperty); }
            set { SetValue(NormalFontSizeProperty, value); }
        }

        public int LargeFontSize
        {
            get { return (int)GetValue(LargeFontSizeProperty); }
            set { SetValue(LargeFontSizeProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public VerticalAlignment VerticalTextAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        /// <summary>
        /// Called when a property changes on the navigation proxy. Since this is a hot path, it's cheaper 
        /// to implement these events manually instead of binding via the triggers system in xaml.
        /// </summary>
        private void OnProxyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.Equals("IsSnapped", e.PropertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (this.proxy.IsSnapped)
                {
                    this.textBlock.SetValue(TextBlock.FontSizeProperty, this.SnappedFontSize);
                    this.textBlock.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.WordEllipsis);
                    this.textBlock.SetValue(TextBlock.TextWrappingProperty, TextWrapping.NoWrap);
                }
                else
                {
                    this.textBlock.SetValue(TextBlock.FontSizeProperty, this.NormalFontSize);
                    this.textBlock.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.None);
                    this.textBlock.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                }
            }
        }

        /// <summary>
        /// If there is a command bound to this text block, invoke it.
        /// </summary>
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (this.Command != null && this.Command.CanExecute(this.CommandParameter))
            {
                this.Command.Execute(this.CommandParameter);
            }
        }
    }
}
