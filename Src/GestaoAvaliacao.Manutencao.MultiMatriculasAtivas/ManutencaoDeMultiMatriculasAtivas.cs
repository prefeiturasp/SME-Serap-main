using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Alunos;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Matriculas;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Alunos;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Matriculas;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas
{
    internal partial class ManutencaoDeMultiMatriculasAtivas : Form
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly IManutencaoDeMultiMatriculasAtivasServices _manutencaoDeMultiMatriculasAtivasServices;

        private IEnumerable<Aluno> _alunosComMultiMatriculasAtivas;

        public ManutencaoDeMultiMatriculasAtivas()
        {
            _alunoRepository = new AlunoRepository();
            _matriculaRepository = new MatriculaRepository();
            _manutencaoDeMultiMatriculasAtivasServices = new ManutencaoDeMultiMatriculasAtivasServices(new DateTime(2020, 8, 4));

            InitializeComponent();
            btnAjustar.Enabled = false;
        }

        private async void btnBuscarAlunos_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtAno.Text))
            {
                MessageBox.Show("O ano deve ser informado.");
                return;
            }

            if(!int.TryParse(txtAno.Text, out var ano))
            {
                MessageBox.Show("O ano informado é inválido.");
                return;
            }

            _alunosComMultiMatriculasAtivas = await _alunoRepository.GetAlunosComMaisDeUmaMatriculaAtiva(ano);
            if(!_alunosComMultiMatriculasAtivas?.Any() ?? true)
            {
                MessageBox.Show("Não foi encontrado nenhum aluno com mais de uma matricula ativa.");
                btnAjustar.Enabled = false;
                btnAjustarSelecionado.Enabled = false;
                return;
            }

            _alunosComMultiMatriculasAtivas = _alunosComMultiMatriculasAtivas.OrderBy(x => x.Matricula).ToList();
            dtAlunos.DataSource = _alunosComMultiMatriculasAtivas;
            dtAlunos.Refresh();
            lblAlunos.Text = $"Quantidade de alunos: {dtAlunos.RowCount}";
            btnAjustar.Enabled = true;
            btnAjustarSelecionado.Enabled = true;
        }

        private async void dtAlunos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!int.TryParse(txtAno.Text, out var ano))
            {
                MessageBox.Show("O ano informado é inválido.");
                return;
            }

            LimparGridDeMatriculasAtivas();
            if (dtAlunos.RowCount <= 0) return;

            long.TryParse(dtAlunos.Rows[e.RowIndex].Cells["AlunoId"].Value.ToString(), out var alunoId);
            if (alunoId <= 0) return;

            var matriculasAtivas = await _matriculaRepository.GetMatriculasAtivasDoAluno(alunoId, ano);
            if(!matriculasAtivas?.Any() ?? true)
            {
                MessageBox.Show("Nenhuma matrícula ativa encontrada para este aluno.");
                return;
            }

            dtMatriculasAtivas.DataSource = matriculasAtivas;
            dtMatriculasAtivas.Refresh();
        }

        private void LimparGridDeMatriculasAtivas()
        {
            dtMatriculasAtivas.DataSource = null;
            dtMatriculasAtivas.Rows.Clear();
        }

        private void btnFechar_Click(object sender, EventArgs e) => Close();

        private async void btnAjustar_Click(object sender, EventArgs e)
        {
            if (!_alunosComMultiMatriculasAtivas?.Any() ?? true) return;

            if (!int.TryParse(txtAno.Text, out var ano))
            {
                MessageBox.Show("O ano informado é inválido.");
                return;
            }

            if (MessageBox.Show("Ao confirmar esta ação todas as provas respondidas pelos alunos nas turmas consideradas incorretas serão ajustadas automaticamente. Deseja continuar?", "Ajuste de matrículas", MessageBoxButtons.YesNo) == DialogResult.No) 
                return;

            AtivarDesativarControlesDaTela(false);
            pgbAjustarMatriculas.Value = 0;
            pgbAjustarMatriculas.Maximum = _alunosComMultiMatriculasAtivas.Count();

            await _manutencaoDeMultiMatriculasAtivasServices.AjustarProvasEDesabilitarMatriculasIncorretas(ano, _alunosComMultiMatriculasAtivas, () => { pgbAjustarMatriculas.Value++; });

            MessageBox.Show("Ajuste realizado!");
            AtivarDesativarControlesDaTela(true);
        }

        private async void btnAjustarSelecionado_Click(object sender, EventArgs e)
        {
            if (!_alunosComMultiMatriculasAtivas?.Any() ?? true) return;

            if (!int.TryParse(txtAno.Text, out var ano))
            {
                MessageBox.Show("O ano informado é inválido.");
                return;
            }

            if (MessageBox.Show("Ao confirmar esta ação todas as provas respondidas pelo aluno selecionado nas turmas consideradas incorretas serão ajustadas automaticamente. Deseja continuar?", "Ajuste de matrículas", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            long.TryParse(dtAlunos.SelectedRows[0].Cells["AlunoId"].Value.ToString(), out var alunoId);
            if (alunoId <= 0) return;

            AtivarDesativarControlesDaTela(false);
            var aluno = _alunosComMultiMatriculasAtivas.Where(x => x.AlunoId == alunoId);
            pgbAjustarMatriculas.Value = 0;
            pgbAjustarMatriculas.Maximum = aluno.Count();

            await _manutencaoDeMultiMatriculasAtivasServices.AjustarProvasEDesabilitarMatriculasIncorretas(ano, aluno, () => { pgbAjustarMatriculas.Value++; });

            MessageBox.Show("Ajuste realizado!");
            AtivarDesativarControlesDaTela(true);
        }

        private void AtivarDesativarControlesDaTela(bool ativar)
        {
            btnBuscarAlunos.Enabled = ativar;
            btnFechar.Enabled = ativar;
            btnAjustar.Enabled = ativar;
            btnAjustarSelecionado.Enabled = ativar;
            dtAlunos.Enabled = ativar;
        }
    }
}
