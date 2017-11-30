using Agent.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgentController
{
    public partial class CommandForm : Form
    {
        public CommandForm()
        {
            InitializeComponent();

            DataTable table = new DataTable();
            table.Columns.Add("Property");
            table.Columns.Add("Value");
            dgvCommand.DataSource = table;
        }

        private void CommandForm_Load(object sender, EventArgs e)
        {
            var commands = GetCommandTypes();
            for(int i = 0; i < commands.Count; i++) {
                cbCommands.Items.Add(commands[i]);
            }
        }

        private List<Type> GetCommandTypes()
        {
            List<Type> types = new List<Type>(Assembly.GetAssembly(typeof(Command)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Command))));
            return types;
        }

        private object CreateInstance()
        {
            Type t = (Type)cbCommands.SelectedItem;
            return Activator.CreateInstance(t, null);
        }

        private void cbCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            var c = (Command)CreateInstance();

            FillTable(c);
        }

        private void FillTable(object c)
        {
            DataTable table = dgvCommand.DataSource as DataTable;
            table.Clear();

            var properties = c.GetType().GetProperties();

            for(int i = 0; i < properties.Length; i++) {
                DataRow dr = table.NewRow();
                dr["Property"] = properties[i].Name;
                dr["Value"] = properties[i].GetValue(c);
                table.Rows.Add(dr);
            }
        }

        private void FillObject(object c)
        {
            DataTable table = dgvCommand.DataSource as DataTable;
            var properties = c.GetType().GetProperties();

            foreach(DataRow dr in table.Rows) {
                for(int i = 0; i < properties.Length; i++) {
                    if(dr["Property"].Equals(properties[i].Name)) {
                        var v = dr["Value"];
                        object o = null;

                        if(properties[i].PropertyType == typeof(string)) {
                            o = v.ToString();
                            properties[i].SetValue(c, o);
                            continue;
                        }

                        var tryParseMethod = properties[i].PropertyType.GetMethod("TryParse", new Type[] { typeof(string), properties[i].PropertyType.MakeByRefType() });
                        if((bool)tryParseMethod.Invoke(c, new object[] { v, o })) {
                            properties[i].SetValue(c, o);
                            continue;
                        }

                        var parseMethod = properties[i].PropertyType.GetMethod("Parse", new Type[] { typeof(string) });
                        o = parseMethod.Invoke(c, new object[] { v });
                        properties[i].SetValue(c, o);
                    }
                }
            }
        }

        private void btnToJson_Click(object sender, EventArgs e)
        {
            var c = CreateInstance();
            FillObject(c);

            tbCommand.Text = CommandHandler.CommandToString(c as Command);
        }

        private void btnToObject_Click(object sender, EventArgs e)
        {
            var c = CommandHandler.GetCommand(tbCommand.Text);
            FillTable(c);
        }
    }
}
