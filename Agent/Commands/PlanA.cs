using Agent.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            DuplicateToAgent(e.contact);
        }

        private void HandleReceivedCommand(object sender, CommandEventArgs e)
        {
            var packageReceived = e.command as PackageReceived;
            if(packageReceived != null) {
                ExecuteSelfOnAgent(e.command.Source);
            }
        }

        private void DuplicateToAgent(AgentContact contact)
        {
            var c = PrepareBreedCommand(contact, Agent.Contact);
            new Sender(Agent, contact, c).Send(true);
        }

        private void ExecuteSelfOnAgent(AgentContact contact)
        {
            var c = new Execute() { command = Execute.MyExecuteCommand };
            new Sender(Agent, contact, c).Send(true);
        }

        public static Command PrepareBreedCommand(AgentContact receiver, AgentContact sender)
        {
            var s = new Send() {
                ip = receiver.ip,
                port = receiver.port,
                message = CommandHandler.CommandToString(new Send() {
                    ip = sender.ip,
                    port = sender.port,
                    message = CommandHandler.CommandToString(new Duplicate() { ip = receiver.ip, port = receiver.port })
                })
            };

            return s;
        }
    }
}
