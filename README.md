# SignalBooster

## Tools & IDE

- **IDE:** Visual Studio Code (VSCode)
- **AI Development Tools:** GitHub Copilot, DeepSeek, ChatGPT

## Assumptions

- Input files (physician notes) are available in a folder accessible to the program at runtime.
- File path and content type are passed as command-line parameters.
- The API URL for submitting orders is currently hardcoded in the driver program, but could be moved to configuration.
- No authentication is required for the HTTP endpoint.
- The code expects the physician note to be in a plain text format.

## Limitations & Future Improvements

- Only a limited set of DME device types and qualifiers are supported in the processor.
- The extraction logic is rule-based and may not handle all real-world note variations.
- Future improvements could include:
  - Adding an LLM-based data extraction module for more robust parsing.
  - Supporting additional DME device types and qualifiers.
  - Making the API endpoint and authentication configurable.
  - Accepting multiple input formats (e.g., JSON-wrapped notes).

## How to Run the SignalBooster Program

1. **Restore dependencies:**  
   Run `dotnet restore` in the SignalBooster project directory.

2. **Build the project:**  
   Run `dotnet build`.

3. **Run the program:**  
   ```
   dotnet run [NoteFilePath] [noteContentType]
   ```
   - Replace `[noteFilePath]` with the full physical path to your physician note file.
   - Replace `[noteContentType]` with the content type (e.g., `text` or `json`).


   ```
   dotnet run "/Users/xxx/SignalBooster/physician_note1.txt" "text"
   ```

   ```
   dotnet run "/Users/xxx/SignalBooster/physician_note2.txt" "json"
   ```

## How to Run the SignalBooster Program

1. **Restore dependencies:**  
   Run `dotnet restore` in the SignalBooster project directory.

2. **Build the project:**  
   Run `dotnet build`.

3. **Run tests:**  
   ```
   dotnet test
   ```

