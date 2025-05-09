window.handleFileDrop = (e) => {
    // This function will be used for handling file drop events
    // For security reasons, Blazor's drag and drop API is limited
    // This JS interop helps bridge that gap
    
    // Prevent default browser behavior
    e.preventDefault();
    
    // In a complete implementation, we would transfer the files
    // to Blazor's InputFile component. For this demo, we'll
    // simply trigger a click on the hidden file input
    
    // Find the input file element
    const inputFile = document.querySelector('.input-file');
    if (inputFile) {
        inputFile.click();
    }
};