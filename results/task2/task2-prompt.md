Prompt:

> Imagine you are an expert technical writer working with a C# file. You have an example of a Markdown documentation provided to you in a 'task-2.documentationExample.md' file. Your task is to generate comprehensive Markdown documentation for the provided C# file using strictly the the format, sections and conventions demonstrated in the provided example Markdown file. Use Markdown best practices when needed if it doesn't interfere with the structure of the provided example file. Provide only generated Markdown file without conversational text.

The prompt worked correctly with Cursor. However, there were issues with Gemini and ChatGPT. The generation went correctly, however, all the sections starting from API Example (cURL) were separated from the initial file. I attempted to identify the issue by asking the following:

> Some sections like error codes and notes are still outside the generated file. Is it an issue with your visualizing or an error of the prompt?

To which ChatGPT provided the following answer:

> ✅ Root of the Problem:
> The closing triple backticks (```markdown or ```) used to wrap the entire Markdown document were placed too early, which visually made the sections appear outside the file. This is a presentation mistake — not a logic or prompt error.

Since outside of this issue the results of the generation were pretty much the same I will not provide them with the answer. However, I had to note this because even with the identified issue neither Gemini nor ChatGPT could fix it when prompted to.

