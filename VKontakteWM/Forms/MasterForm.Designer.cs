using System;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Forms
{
    partial class MasterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            DebugHelper.WriteLogEntry("*** Program tick count: " + (Environment.TickCount - _startTick));
            DebugHelper.WriteLogEntry("*** Cache read tick count: " + Cache.FileUsingReadTicks);
            DebugHelper.WriteLogEntry("*** Cache write tick count: " + Cache.FileUsingWriteTicks);
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //this.components = new System.ComponentModel.Container();
            //this.mainMenu1 = new System.Windows.Forms.MainMenu();
            //this.Menu = this.mainMenu1;
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            //this.Text = "";
            //this.AutoScroll = true;
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.SuspendLayout();
            // 
            // MasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Menu = this.mainMenu1;
            this.Name = "MasterForm";
            this.Text = "";
            this.ResumeLayout(false);

        }

        #endregion
    }
}