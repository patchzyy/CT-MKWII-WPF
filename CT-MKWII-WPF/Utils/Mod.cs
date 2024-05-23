﻿namespace CT_MKWII_WPF.Utils;

using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.ComponentModel;

public class Mod : INotifyPropertyChanged
{
    private bool _isEnabled;
    private string _title;
    private string _author;

    public bool IsEnabled
    {
        get { return _isEnabled; }
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
    }

    public string Title
    {
        get { return _title; }
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }
    
    public string Author
    {
        get { return _author; }
        set
        {
            if (_author != value)
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ModData
{
    public bool IsEnabled { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
}