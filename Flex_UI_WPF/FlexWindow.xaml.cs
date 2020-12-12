﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SimpleNLG;
using FlexibleRealization;
using FlexibleRealization.UserInterface;
using Flex.UserInterface.ViewModels;

namespace Flex.UserInterface
{
    /// <summary>Interaction logic for ParseAndRealizeWindow.xaml</summary>
    public partial class FlexWindow : Window
    {
        public FlexWindow()
        {
            InitializeComponent();
            GraphEditor.ElementBuilderSelected += GraphEditor_ElementBuilderSelected;
            GraphEditor.RealizationFailed += GraphEditor_RealizationFailed;
            GraphEditor.TextRealized += GraphEditor_TextRealized;
        }

        private void GraphEditor_ElementBuilderSelected(ElementBuilder selectedBuilder)
        {
            switch (selectedBuilder)
            {
                case WordElementBuilder word:
                    WordSelectorConfiguratior.ViewModel = WordSelectorViewModel.For(word);
                    ShowWordConfigurator();
                    break;
                default:
                    HideConfigurators();
                    break;
            }

            void HideConfigurators()
            {
                WordSelectorConfiguratior.Visibility = Visibility.Collapsed;
            }

            void ShowWordConfigurator()
            {
                WordSelectorConfiguratior.Visibility = Visibility.Visible;
            }
        }

        /// <summary>This event handler is called when the GraphEditor has successfully realized some text</summary>
        private void GraphEditor_TextRealized(string realizedText)
        {
            realizedTextBox.Background = Brushes.WhiteSmoke;
            realizedTextBox.Text = realizedText;
        }

        /// <summary>This event handler is called when the GraphEditor has tried to realize an IElementTreeNode, but failed</summary>
        private void GraphEditor_RealizationFailed(IElementTreeNode failed)
        {
            realizedTextBox.Background = RealizeFailedBrush;
            realizedTextBox.Text = "";
        }

        /// <summary>A color representing that realization has failed</summary>
        private static Brush RealizeFailedBrush = new SolidColorBrush(Color.FromArgb(100, 254, 0, 0));

        protected override void OnClosing(CancelEventArgs e)
        {
            GraphEditor.ElementBuilderSelected -= GraphEditor_ElementBuilderSelected;
            GraphEditor.RealizationFailed -= GraphEditor_RealizationFailed;
            GraphEditor.TextRealized -= GraphEditor_TextRealized;
        }

        public Visibility WordAlternativesVisibility { get; private set; }

        /// <summary>When the user changes a setting for the CoreNLP server, save its settings</summary>
        private void CoreNLP_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => Stanford.CoreNLP.Properties.Settings.Default.Save();

        /// <summary>When the user changes a setting for the SimpleNLG server, save its settings</summary>
        private void SimpleNLG_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => SimpleNLG.Properties.Settings.Default.Save();

        /// <summary>When the user changes a setting for the SimpleNLG server, save its settings</summary>
        private void WordNet_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => WordNet.Linq.Properties.Settings.Default.Save();

        //private void GraphEditor_ElementBuilderSelected(ElementBuilder selectedBuilder) => RealizeAndDisplay(selectedBuilder);

        /// <summary>If there's text in the inputTextBox, parse it</summary>
        private void parseButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputTextBox.Text.Length > 0) HandleTextInput(inputTextBox.Text);
        }

        /// <summary>Send <paramref name="text"/> to the GraphEditor</summary>
        private void HandleTextInput(string text) => GraphEditor.ParseText(text);

        /// <summary>The user has entered some text in the inputTextBox</summary>
        private void inputTextBox_TextInput(object sender, TextCompositionEventArgs e) => HandleTextInput(e.Text);

        private void showVariationsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (IElementTreeNode eachRealizableVariation in GraphEditor.SelectedBuilder.GetRealizableVariations())
            {
                TryToRealize(eachRealizableVariation);
            }
        }

        /// <summary>Try to transform <paramref name="editableTree"/> into realizable form and if successful, try to realize it</summary>
        private void TryToRealize(IElementTreeNode editableTree)
        {
            RealizationResult result = editableTree.Realize();
            switch (result.Outcome)
            {
                case RealizationOutcome.Success:
                    Console.WriteLine(result.Realized);
                    break;
                default: break;
            }
        }

    }
}
