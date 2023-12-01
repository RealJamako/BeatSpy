﻿using System.Windows;
using BeatSpy.Commands.Base;

namespace BeatSpy.Commands;

internal class MinimizeApplicationCommand : CommandBase
{
    public override void Execute(object? parameter)
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }
}