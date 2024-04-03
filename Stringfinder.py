import os
import concurrent.futures
import chardet

# Define the string to search for
SEARCH_STRING = "6.0.8"

# Define the directory to search through
DIRECTORY = "C:\\Users\\patchzy\\AppData\\Roaming\\Dolphin Emulator\\Load\\Riivolution\\RetroRewind6"

def search_in_file(file_path):
    try:
        with open(file_path, 'rb') as f:
            data = f.read()
            # Use chardet to detect the encoding
            encoding = chardet.detect(data)['encoding']
            # If the encoding is detected, decode the data to a string, ignoring errors
            if encoding:
                try:
                    data = data.decode(encoding, errors='ignore')
                except UnicodeDecodeError:
                    # If there's a decoding error even with ignoring errors, skip this file
                    print(f"Decoding error (ignored) in {file_path}")
                    return None
                if SEARCH_STRING in data:
                    return file_path
    except Exception as e:
        # Handle exceptions (like issues opening the file)
        print(f"Error processing {file_path}: {e}")
    return None


def search_files(directory):
    files_found = []
    with concurrent.futures.ThreadPoolExecutor() as executor:
        # Walk through the directory, and list all files
        files_to_search = [os.path.join(root, file)
                           for root, dirs, files in os.walk(directory)
                           for file in files]
        # Use map to apply the search_in_file function to all files
        results = executor.map(search_in_file, files_to_search)
        # Filter out None results and append valid findings
        files_found.extend(filter(None, results))
    return files_found

if __name__ == "__main__":
    found_files = search_files(DIRECTORY)
    print("Files containing the search string:")
    for file in found_files:
        print(file)
