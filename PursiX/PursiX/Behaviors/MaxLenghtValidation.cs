using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PursiX.Behaviors
{
    class MaxLenghtValidation : Behavior<Entry>
    {
        //https://www.c-sharpcorner.com/article/input-validation-in-xamarin-forms-behaviors/

        public int MaxLength { get; set; }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= OnEntryTextChanged;
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;

            if (entry.Text.Length > this.MaxLength)
            {
                string entryText = entry.Text;
                entry.TextChanged -= OnEntryTextChanged;
                entry.Text = e.OldTextValue;
                entry.TextChanged += OnEntryTextChanged;
            }
        }
    }
}
