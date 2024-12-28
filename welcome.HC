
U0 WelcomeMsgWin() {
    // Create a new window with a title
    Win *win;
    win = WinNew("Welcome to Nova-S6", 500, 300, WN_CENTERED);

    // Set window colors (optional, customize as desired)
    win->bgColor = RGB(0, 0, 128); // Background color - dark blue
    win->fgColor = RGB(255, 255, 255); // Text color - white

    // Add text content to the window
    PrintAt(win, 10, 10, "🔥 Welcome to Nova-S6! 🔥");
    PrintAt(win, 10, 30, "This is a divine place for exploration.");
    PrintAt(win, 10, 50, "Feel free to code, create, and worship. 🙏");

    // Optionally add a button or menu for interaction
    PrintAt(win, 10, 90, "[Press ESC to exit]");

    // Keep the window open until the user presses ESC
    U0 KeyPress;
    Do {
        KeyPress = GetKey();
    } While (KeyPress != ESC);

    // Close the window after exiting
    WinClose(win);
}

WelcomeMsgWin;
