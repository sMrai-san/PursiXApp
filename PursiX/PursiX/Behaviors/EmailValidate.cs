﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace PursiX.Behaviors
{
    class EmailValidate : Behavior<Entry>
        {
        //https://www.c-sharpcorner.com/article/input-validation-in-xamarin-forms-behaviors/

        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";


            protected override void OnAttachedTo(Entry bindable)
            {
                bindable.TextChanged += HandleTextChanged;
                base.OnAttachedTo(bindable);
            }

            void HandleTextChanged(object sender, TextChangedEventArgs e)
            {
                bool IsValid = false;
                IsValid = (Regex.IsMatch(e.NewTextValue, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
                ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
            }

            protected override void OnDetachingFrom(Entry bindable)
            {
                bindable.TextChanged -= HandleTextChanged;
                base.OnDetachingFrom(bindable);
            }
    }


    
}
