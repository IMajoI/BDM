using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BDM_Form
{    
    public class BDM_ObjectTemplates
    {
        public BDM_AppTemplates TIATemplates;
        public BDM_AppTemplates S7TEmplates;
        public BDM_AppTemplates ATemplates;
        public BDM_AppTemplates PTemplates;
        public BDM_ObjectTemplates()
        {
            TIATemplates =new BDM_AppTemplates();
            S7TEmplates = new BDM_AppTemplates();
            ATemplates = new BDM_AppTemplates();
            PTemplates = new BDM_AppTemplates();
        }
    }

    public class BDM_AppTemplates
    {
        
        private BindingList<BDM_ObjTmpl> templates;
        public BindingList<BDM_ObjTmpl> Templates { get { return templates; } }

        public BDM_AppTemplates()
        {
            templates = new BindingList<BDM_ObjTmpl>();            
        }
        public void AddTemplate(string TemplateName)
        {
            BDM_ObjTmpl BDM_T = new BDM_ObjTmpl(TemplateName);           
            this.templates.Add(BDM_T);
        }
    } 

    public class BDM_ObjTmpl
    {
        public string TmplName { get; set; }
        public BindingList<BDM_FC> BDMFCs;           

        public BDM_ObjTmpl(string TemplateName)
        {
            TmplName = TemplateName;
            BDMFCs = new BindingList<BDM_FC>();          
        }
        public void AddFC(string FCName)
        {
            BDM_FC BDM_T = new BDM_FC(FCName);
            this.BDMFCs.Add(BDM_T);
        }
    }   
    
    public class BDM_FC
    {
        public string FCName { get; set; }
        public BindingList<BDM_ObjTmplPar> ObjectParmeters;
        public BindingList<BDM_ObjTmplPar> ObjectParOptional;

        public BDM_FC(string FCName)
        {
            this.FCName = FCName;
            ObjectParmeters = new BindingList<BDM_ObjTmplPar>();
        }
    } 

    public class BDM_ObjTmplPar
    {   
        public string BDMObjParType { get; set; }
        public string BDMObjParName { get; set; }
        public string BDMObjParValue { get; set; }  
    }
}
