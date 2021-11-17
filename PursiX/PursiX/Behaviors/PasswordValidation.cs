﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Text.RegularExpressions;

namespace PursiX.Behaviors
{
    class PasswordValidation:Behavior<Entry>
    {
        //https://www.c-sharpcorner.com/article/input-validation-in-xamarin-forms-behaviors/

        const string passwordRegex = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";


        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsValid = false;
            IsValid = (Regex.IsMatch(e.NewTextValue, passwordRegex));
            ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
}
