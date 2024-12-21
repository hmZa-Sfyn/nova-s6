# CLI Command Documentation

## Builtin Commands

### Echo Commands
- **echo**: Echoes input as output.
  ```sh
  echo [text]
  ```
  Prints the provided text on the same line.
- **echoln**: Echoes input as output on separate lines.
  ```sh
  echoln [text]
  ```
  Prints the provided text on separate lines.

### Mind Commands
- **mind**: Manages MindStorage.
  - **-add**: Adds data points.
    ```sh
    mind -add [data1] [data2] ...
    ```
  - **-get**: Retrieves a data point by index.
    ```sh
    mind -get [index]
    ```
  - **-list**: Lists all stored data points.
    ```sh
    mind -list
    ```

### Register Commands
- **reg**: Manages RegisterStorage.
  - **>>set**: Sets a key-value pair.
    ```sh
    reg>>set>>[key]>>[value]
    ```
  - **>>get**: Retrieves the value for a key.
    ```sh
    reg>>get>>[key]
    ```
  - **>>list**: Lists all key-value pairs.
    ```sh
    reg>>list
    ```

### Environment Variable Commands
- **$$env**: Manages environment variables.
  - **>>[key]>>[value]**: Sets an environment variable.
    ```sh
    $$env>>[key]>>[value]
    ```
  - **>>$rem>>[key]**: Removes an environment variable.
    ```sh
    $$env>>$rem>>[key]
    ```
  - **>>$all**: Lists all environment variables.
    ```sh
    $$env>>$all
    ```

### Help Command
- **help**: Displays help information for built-in commands.
  ```sh
  help
  ```

---

## New Commands

### Network Commands
- **net**: Manages network utilities.
  - **ping**: Pings a specified host.
    ```sh
    net ping [host]
    ```
  - **status**: Checks network status.
    ```sh
    net status
    ```
  - **config**: Displays or updates network configuration.
    ```sh
    net config [options]
    ```
  - **scan**: Scans a specified network range.
    ```sh
    net scan [range]
    ```

### Time Commands
- **time**: Displays or sets the system time.
  - **show**: Shows the current system time.
    ```sh
    time show
    ```
  - **set**: Sets the system time.
    ```sh
    time set [HH:MM:SS]
    ```

### File Commands
- **file**: Performs file operations.
  - **read**: Reads the contents of a file.
    ```sh
    file read [path]
    ```
  - **write**: Writes data to a file.
    ```sh
    file write [path] [data]
    ```
  - **delete**: Deletes a specified file.
    ```sh
    file delete [path]
    ```

### Directory Commands
- **dir**: Manages directories.
  - **list**: Lists contents of a directory.
    ```sh
    dir list [path]
    ```
  - **create**: Creates a new directory.
    ```sh
    dir create [path]
    ```
  - **delete**: Deletes a specified directory.
    ```sh
    dir delete [path]
    ```

### Other Commands
- **sysinfo**: Displays system information.
  ```sh
  sysinfo
  ```
- **clear**: Clears the terminal screen.
  ```sh
  clear
  ```
- **history**: Displays command history.
  ```sh
  history
  ```
