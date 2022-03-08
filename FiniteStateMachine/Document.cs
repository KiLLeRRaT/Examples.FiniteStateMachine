using Stateless;

namespace DocumentWorkflow.FiniteStateMachine
{
	public class Document
	{
		private enum State
		{
			Draft,
			Review,
			ChangesRequested,
			SubmittedToClient,
			Approved,
			Declined
		}

		private enum Triggers
		{
			UpdateDocument,
			BeginReview,
			ChangedNeeded,
			Accept,
			Reject,
			Submit,
			Decline,
			RestartReview,
			Approve,
		}

		private readonly StateMachine<State, Triggers> machine;

		private readonly StateMachine<State, Triggers>.TriggerWithParameters<string> changedNeededParameters;

		private async Task OnDraftEntryAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the draft stage");
		}

		private async Task OnDraftExitAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the draft stage");
		}

		private async Task OnReviewEntryAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the review stage");
		}

		private async Task OnReviewExitAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the review stage");
		}

		private async Task OnChangesRequestedEntryAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the changes requested stage");
		}

		private async Task OnChangesRequestedExitAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the changes requested stage");
		}

		private async Task OnSubmittedToClientExitAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the submitted to client stage");
		}

		private async Task OnSubmittedToClientEnterAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the approved stage");
		}

		private async Task OnDeclinedExitAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has now left the declined stage");
		}

		private async Task OnDeclinedEnterAsync()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal is now in the draft stage");
		}

		private async Task OnApprovedEnter()
		{
			// await notificationService.SendUpdateAsync(Priority.Verbose, "The proposal has been approved");
		}

		public Document()
		{
			// We can create the FSM with state stored in a file, DB, ORM wherever. In that case we'd need to create a factory
			// so the constructor isn't doing long/async work.
			//machine = new StateMachine<State, Triggers>(() => state, s => state = s);

			machine = new StateMachine<State, Triggers>(State.Draft);

			machine.Configure(State.Draft)
				.PermitReentry(Triggers.UpdateDocument)
				.Permit(Triggers.BeginReview, State.Review)
				.OnEntryAsync(OnDraftEntryAsync)
				.OnExitAsync(OnDraftExitAsync);

			changedNeededParameters = machine.SetTriggerParameters<string>(Triggers.ChangedNeeded);

			machine.Configure(State.Review)
				.Permit(Triggers.ChangedNeeded, State.ChangesRequested)
				.Permit(Triggers.Submit, State.SubmittedToClient)
				.OnEntryAsync(OnReviewEntryAsync)
				.OnExitAsync(OnReviewExitAsync);

			machine.Configure(State.ChangesRequested)
				.Permit(Triggers.Reject, State.Review)
				.Permit(Triggers.Accept, State.Draft)
				.OnEntryAsync(OnChangesRequestedEntryAsync)
				.OnExitAsync(OnChangesRequestedExitAsync);

			machine.Configure(State.SubmittedToClient)
				.Permit(Triggers.Approve, State.Approved)
				.Permit(Triggers.Decline, State.Declined)
				.OnEntryAsync(OnSubmittedToClientEnterAsync)
				.OnExitAsync(OnSubmittedToClientExitAsync);

			machine.Configure(State.Declined)
				.Permit(Triggers.RestartReview, State.Review)
				.OnEntryAsync(OnDeclinedEnterAsync)
				.OnExitAsync(OnDeclinedExitAsync);

			machine.Configure(State.Approved)
				.OnEntryAsync(OnApprovedEnter);
		}
	}
}
