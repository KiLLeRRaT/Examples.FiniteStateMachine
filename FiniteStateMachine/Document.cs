using System;
using Stateless;

namespace DocumentWorkflow.FiniteStateMachine
{
	public class Document
	{
		public enum State
		{
			Draft,
			Review,
			ChangesRequested,
			SubmittedToClient,
			Approved,
			Declined
		}

		public enum Triggers
		{
			UpdateDocument,
			BeginReview,
			ChangesNeeded,
			Accept,
			Reject,
			Submit,
			Decline,
			RestartReview,
			Approve,
		}

		public readonly StateMachine<State, Triggers> Machine;

		private readonly StateMachine<State, Triggers>.TriggerWithParameters<string> changedNeededParameters;

		private async Task OnDraftEntryAsync()
		{
			Console.WriteLine("Document is in Draft state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the draft stage");
		}

		private async Task OnDraftExitAsync()
		{
			Console.WriteLine("Document is no longer in Draft state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the draft stage");
		}

		private async Task OnReviewEntryAsync()
		{
			Console.WriteLine("Document is in Review state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the review stage");
		}

		private async Task OnReviewExitAsync()
		{
			Console.WriteLine("Document is no longer in Review state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the review stage");
		}

		private async Task OnChangesRequestedEntryAsync()
		{
			Console.WriteLine("Document is in Changes Requested state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the changes requested stage");
		}

		private async Task OnChangesRequestedExitAsync()
		{
			Console.WriteLine("Document is no longer in Changes Requested state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the changes requested stage");
		}

		private async Task OnSubmittedToClientExitAsync()
		{
			Console.WriteLine("Document is no longer in Submitted to Client state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the submitted to client stage");
		}

		private async Task OnSubmittedToClientEnterAsync()
		{
			Console.WriteLine("Document is in Submitted to Client state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the approved stage");
		}

		private async Task OnDeclinedExitAsync()
		{
			Console.WriteLine("Document is no longer in Declined state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the declined stage");
		}

		private async Task OnDeclinedEnterAsync()
		{
			Console.WriteLine("Document is in Declined state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the draft stage");
		}

		private async Task OnApprovedEnter()
		{
			Console.WriteLine("Document is in Approved state");
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has been approved");
		}

		public async Task UpdateDocumentAsync() => await Machine.FireAsync(Triggers.UpdateDocument);

		public async Task BeginReviewAsync() => await Machine.FireAsync(Triggers.BeginReview);

		public async Task MakeChangeAsync(string change) => await Machine.FireAsync(changedNeededParameters, change);

		public async Task AcceptAsync() => await Machine.FireAsync(Triggers.Accept);

		public async Task RejectAsync() => await Machine.FireAsync(Triggers.Reject);

		public async Task SubmitAsync() => await Machine.FireAsync(Triggers.Submit);

		public async Task RestartReviewAsync() => await Machine.FireAsync(Triggers.RestartReview);

		public async Task ApproveAsync() => await Machine.FireAsync(Triggers.Approve);

		public async Task DeclineAsync() => await Machine.FireAsync(Triggers.Decline);

		public State CurrentState { get { return Machine.State; } }

		public Document()
		{
			// We can create the FSM with state stored in a file, DB, ORM wherever. In that case we'd need to create a factory
			// so the constructor isn't doing long/async work.
			//machine = new StateMachine<State, Triggers>(() => state, s => state = s);

			Machine = new StateMachine<State, Triggers>(State.Draft);

			Machine.Configure(State.Draft)
				.PermitReentry(Triggers.UpdateDocument)
				.Permit(Triggers.BeginReview, State.Review)
				.OnEntryAsync(OnDraftEntryAsync)
				.OnExitAsync(OnDraftExitAsync);

			changedNeededParameters = Machine.SetTriggerParameters<string>(Triggers.ChangesNeeded);

			Machine.Configure(State.Review)
				.Permit(Triggers.ChangesNeeded, State.ChangesRequested)
				.Permit(Triggers.Submit, State.SubmittedToClient)
				.OnEntryAsync(OnReviewEntryAsync)
				.OnExitAsync(OnReviewExitAsync);

			Machine.Configure(State.ChangesRequested)
				.Permit(Triggers.Reject, State.Review)
				.Permit(Triggers.Accept, State.Draft)
				.OnEntryAsync(OnChangesRequestedEntryAsync)
				.OnExitAsync(OnChangesRequestedExitAsync);

			Machine.Configure(State.SubmittedToClient)
				.Permit(Triggers.Approve, State.Approved)
				.Permit(Triggers.Decline, State.Declined)
				.OnEntryAsync(OnSubmittedToClientEnterAsync)
				.OnExitAsync(OnSubmittedToClientExitAsync);

			Machine.Configure(State.Declined)
				.Permit(Triggers.RestartReview, State.Review)
				.OnEntryAsync(OnDeclinedEnterAsync)
				.OnExitAsync(OnDeclinedExitAsync);

			Machine.Configure(State.Approved)
				.OnEntryAsync(OnApprovedEnter);
		}
	}
}
