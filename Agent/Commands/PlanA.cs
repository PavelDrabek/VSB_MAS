using Agent.Communication;
using Agent.Utilities;

namespace Agent.Commands
{
    public class PlanA : Command
    {
        public override void Inject(Agent agent)
        {
            base.Inject(agent);
        }

        public override void ExecuteCommand()
        {
            Agent.AgentContactBook.OnContactAdded += HandleAddedContact;
            Agent.OnCommandReceived += HandleReceivedCommand;

            var contacts = Agent.AgentContactBook.Contacts;
            foreach(var contact in contacts) {
                DuplicateToAgent(contact);
            }
        }

        private void Stop()
        {
            Agent.AgentContactBook.OnContactAdded -= HandleAddedContact;
            Agent.OnCommandReceived -= HandleReceivedCommand;
        }

        private void HandleAddedContact(object sender, AgentContactBook.ContactEventArgs e)
        {
            GetContactsFromAgent(e.contact);
            DuplicateToAgent(e.contact);
        }

        private void HandleReceivedCommand(object sender, CommandEventArgs e)
        {
            var packageReceived = e.command as PackageReceived;
            if(packageReceived != null) {
                for(int i = 0; i < Agent.Config.NumExecute; i++) {
                    ExecuteSelfOnAgent(e.command.Source);
                }
                if(!e.command.Source.tag.Equals(Agent.TAG)) {
                    SendHaltToAgent(e.command.Source);
                }
            }
        }

        private void GetContactsFromAgent(AgentContact contact)
        {
            Debug.Log(string.Format("Plan: getting agents from {0}", contact), Logger.Level.Plan);
            var c = new Agents();
            c.InjectAgentInfo(Agent);
            new Sender(Agent, contact, c).Send(true);
        }

        private void DuplicateToAgent(AgentContact contact)
        {
            Debug.Log(string.Format("Plan: duplicate to {0}", contact), Logger.Level.Plan);
            var c = PrepareBreedCommand(contact, Agent.Contact);
            c.InjectAgentInfo(Agent);
            new Sender(Agent, contact, c).Send(true);
        }

        private void ExecuteSelfOnAgent(AgentContact contact)
        {
            Debug.Log(string.Format("Plan: executing on {0}", contact), Logger.Level.Plan);
            var c = new Execute() { command = Execute.MyExecuteCommand };
            c.InjectAgentInfo(Agent);
            new Sender(Agent, contact, c).Send(true);
        }

        private void SendHaltToAgent(AgentContact contact)
        {
            Debug.Log(string.Format("Plan: halt to {0}", contact), Logger.Level.Plan);
            var c = new Halt();
            c.InjectAgentInfo(Agent);
            new Sender(Agent, contact, c).Send(true);
        }

        public static Command PrepareBreedCommand(AgentContact receiver, AgentContact sender)
        {
            //var s = new Send() {
            //    ip = receiver.ip,
            //    port = receiver.port,
            //    message = CommandHandler.CommandToString(new Send() {
            //        ip = sender.ip,
            //        port = sender.port,
            //        message = CommandHandler.CommandToString(new Duplicate() { ip = receiver.ip, port = receiver.port })
            //    })
            //};

            var s = new Send() {
                ip = sender.ip,
                port = sender.port,
                message = CommandHandler.CommandToString(new Duplicate() { ip = receiver.ip, port = receiver.port })
            };

            return s;
        }
    }
}
