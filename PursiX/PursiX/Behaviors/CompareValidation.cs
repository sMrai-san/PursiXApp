﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PursiX.Behaviors
{
    class CompareValidation:Behavior<Entry>
    {
        //https://www.c-sharpcorner.com/article/input-validation-in-xamarin-forms-behaviors/

        public static BindableProperty TextProperty = BindableProperty.Create<CompareValidation, string>(tc => tc.Text, string.Empty, BindingMode.TwoWay);

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }


        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsValid = false;
            IsValid = e.NewTextValue == Text;

            ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
}
