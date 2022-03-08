using DocumentWorkflow.FiniteStateMachine;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// Create a new document
Document doc = new Document();
Console.WriteLine("Current state: " + doc.CurrentState);

// Go through the states of a document based on user input
while (true)
{
	if (doc.CurrentState == Document.State.Approved)
	{
		Console.WriteLine("Thanks for playing, bye!");
		break;
	}

	Console.WriteLine("Available triggers: " + string.Join(", ", doc.Machine.PermittedTriggers));
	Console.WriteLine("Enter a trigger or \"exit\": ");
	string input = Console.ReadLine();
	if (input == "exit")
	{
		break;
	}

	if (!string.IsNullOrEmpty(input))
	{
		if (Enum.TryParse<Document.Triggers>(input, out Document.Triggers trigger))
		{
		await doc.Machine.FireAsync(trigger);
		}
		else
		{
			Console.WriteLine("Invalid trigger");
		}
	}
	Console.WriteLine("");
}



Console.WriteLine("Done, press any key to quit...");
Console.ReadKey();
