using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement
{
    public class DatasetManager : BaseManager
    {
        public DatasetManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public Dataset GetDataset(int datasetId)
        {
            var items = m_db.Query<ABSMgrConn.TableDataset>(
                "SELECT * FROM dbo.Dataset where dataset_id = @0", datasetId);

            if (items.Count() != 1)
            {
                throw new ApplicationException("Get dataset [" + datasetId.ToString() + "] failed.");
            }

            var dataset = new Dataset(items.Single());
            return dataset;
        }

        public List<Dataset> GetDatasetByProjectId(int projectId)
        {
            var items = m_db.Query<ABSMgrConn.TableDataset>(
                "SELECT * FROM dbo.Dataset WHERE project_id = @0 ORDER BY payment_date DESC", projectId);

            return items.ToList().ConvertAll(x => new Dataset(x)).ToList();
        }

        public List<Dataset> GetDatasetByProjectIds(IEnumerable<int> projectId)
        {
            var datasets = Select<ABSMgrConn.TableDataset, int>("dbo.Dataset", "project_id", projectId, " ORDER BY payment_date DESC");
            return datasets.Select(x => new Dataset(x)).ToList();
        }

        /// <summary>
        /// 根据存续期，取得对应的Dataset
        /// 给出存续期间中任一时间，按照该时间对应的兑付日，取得Dataset
        /// 另：
        /// 给定日期在第一期AsOfDate之前，返回第一期Dataset
        /// 超出存续期间时，返回最后一期Dataset
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Dataset GetDatasetByDurationPeriod(int projectId, DateTime date)
        {
            var items = GetDatasetByProjectId(projectId);
            items.Reverse();
            if (items.Count == 0)
            {
                return null;
            }

            return items.FirstOrDefault(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= date) ?? items.Last();
        }

        public Dataset GetDataset(int projectId, DateTime paymentDate)
        {
            var items = GetDatasetByProjectId(projectId);
            var datasets = items.Where(x => x.PaymentDate.HasValue && x.PaymentDate == paymentDate).ToList();
            CommUtils.Assert(datasets.Count < 2, "Get dataset failed, projectId=[" + projectId + "] paymentDate=[" + paymentDate.ToString() + "]");
            return datasets.Count == 1 ? datasets.First() : null;
        }

        public Dataset GetDatesetByProjectIdAsOfDate(int projectId, string asOfDate)
        {
            var items = m_db.Query<ABSMgrConn.TableDataset>(
                "SELECT * FROM dbo.Dataset where project_id = @0 and as_of_date = @1", projectId, asOfDate);

            if (items.Count() != 1)
            {
                throw new ApplicationException("Get dataset [" + asOfDate + "] failed.");
            }

            var dataset = new Dataset(items.Single());
            return dataset;
        }

        public int DeleteDataset(Dataset dataset)
        {
            return m_db.Delete("Dataset", "dataset_id", dataset.GetTableObject());
        }

        public string GetLatestDatasetFolder(Project project)
        {
            var asOfDate = GetLatestDatasetByProjectId(project.ProjectId).AsOfDate;
            return GetDatasetFolder(project, asOfDate);
        }

        public string GetDatasetFolder(Project project, DateTime asOfDate)
        {
            return GetDatasetFolder(project, asOfDate.ToString("yyyyMMdd"));
        }

        public string GetDatasetFolder(Project project, string asOfDate)
        {
            var rootFolder = WebConfigUtils.RootFolder;
            return rootFolder + project.Model.ModelFolder + "\\" + asOfDate + "\\";
        }

        public string GetTemporaryFolder()
        {
            var rootFolder = WebConfigUtils.RootFolder;
            return rootFolder + "Temporay" + "\\" + UserInfo.UserName;
        }

        public string GetYmlFolder(Project project)
        {
            var rootFolder = WebConfigUtils.RootFolder;
            return System.IO.Path.Combine(rootFolder, project.Model.ModelFolder);
        }

        public string GetYmlFilePath(Project project)
        {
            var ymlFolder = GetYmlFolder(project);
            return System.IO.Path.Combine(ymlFolder, "script.yml");
        }

        public Dataset GetLatestDatasetByProjectId(int projectId)
        {
            var dataset = m_db.Query<ABSMgrConn.TableDataset>(
                "SELECT TOP 1 * FROM dbo.Dataset WHERE project_id = @0 ORDER BY payment_date desc", projectId).FirstOrDefault();

            return new Dataset(dataset);
        }

        public Dataset NewDataset(Dataset dataset)
        {
            var tableDataset = dataset.GetTableObject();
            var datasetId = m_db.Insert("Dataset", "dataset_id", true, tableDataset);
            dataset.DatasetId = (int)datasetId;
            return dataset;
        }

        public List<Note> GetNotes(int projectId)
        {
            var items = m_db.Query<ABSMgrConn.TableNote>(
                "SELECT * FROM dbo.Note WHERE project_id = @0 ORDER BY note_id", projectId);
            return items.ToList().ConvertAll(x => new Note(x)).ToList();
        }

        public List<string> GetNoteNames(int projectId)
        {
            return GetNotes(projectId).ConvertAll(x => x.NoteName).ToList();
        }

        public bool NoteExists(int projectId, int noteId)
        {
            string sql = @"SELECT COUNT(*) FROM dbo.Note WHERE project_id = @0 AND note_id = @1";
            int count = m_db.ExecuteScalar<int>(sql, projectId, noteId);
            return count > 0;
        }

        public Note GetNote(int projectId, int noteId)
        {
            var items = m_db.Query<ABSMgrConn.TableNote>(
                "SELECT * FROM dbo.Note WHERE project_id = @0 AND note_id = @1 ORDER BY note_id",
                projectId, noteId);

            if (items.Count() != 1)
            {
                throw new ApplicationException("Get note [" + projectId.ToString() + "][" + noteId.ToString() + "] failed.");
            }

            return new Note(items.Single());
        }

        public Note NewNote(Note note)
        {
            if (!note.ProjectId.HasValue
                || NoteExists(note.ProjectId.Value, note.NoteId))
            {
                throw new ApplicationException("Add new note [" + note.ProjectId.ToString() + "][" + note.NoteId.ToString() + "] failed.");
            }

            var tableNote = note.GetTableObject();
            var noteId = (int)m_db.Insert("Note", "note_id", true, tableNote);
            note.NoteId = noteId;
            return note;
        }

        public NoteData GetNoteData(int noteId, int datasetId)
        {
            var items = m_db.Query<ABSMgrConn.TableNoteData>(
                "SELECT * FROM dbo.NoteData WHERE note_id = @0 AND dataset_id = @1",
                noteId, datasetId);

            if (items.Count() != 1)
            {
                throw new ApplicationException("Get noteData [" + noteId.ToString() + "][" + datasetId.ToString() + "] failed.");
            }

            return new NoteData(items.Single());
        }

        public List<NoteData> GetNoteDatas(int datasetId)
        {
            var items = m_db.Query<ABSMgrConn.TableNoteData>(
                "SELECT * FROM dbo.NoteData WHERE dataset_id = @0 ORDER BY note_id", datasetId);

            return items.ToList().ConvertAll(x => new NoteData(x)).ToList();
        }

        public bool NoteDataExists(int noteId, int datasetId)
        {
            string sql = @"SELECT COUNT(*) FROM dbo.NoteData WHERE note_id = @0 AND dataset_id = @1";
            int count = m_db.ExecuteScalar<int>(sql, noteId, datasetId);
            return count > 0;
        }

        public NoteData NewNoteData(NoteData noteData)
        {
            if (NoteDataExists(noteData.NoteId, noteData.DatasetId))
            {
                throw new ApplicationException("Add new note [" + noteData.NoteId.ToString() + "][" + noteData.DatasetId.ToString() + "] failed.");
            }

            var tableNoteData = noteData.GetTableObject();
            var id = m_db.Insert("NoteData", "note_data_id", true, tableNoteData);
            noteData.NoteDataId = (int)id;
            return noteData;
        }

        public int UpdateNoteData(NoteData nd)
        {
            var ndTable = nd.GetTableObject();
            return m_db.Update("NoteData", "note_data_id", ndTable, nd.NoteDataId);
        }

        public bool NoteResultExists(int projectId, int datasetId)
        {
            string sql = @"SELECT COUNT(*) FROM dbo.NoteResults WHERE project_id = @0 AND dataset_id = @1";
            int count = m_db.ExecuteScalar<int>(sql, projectId, datasetId);
            return count > 0;
        }

        public NoteResults NewNoteResult(NoteResults noteResult)
        {
            if (NoteDataExists(noteResult.ProjectId, noteResult.DatasetId))
            {
                throw new ApplicationException("Add new note result [" + noteResult.ProjectId.ToString() + "][" + noteResult.DatasetId.ToString() + "] failed.");
            }

            var tableNoteResult = noteResult.GetTableObject();
            var id = m_db.Insert("NoteResults", "result_id", true, tableNoteResult);
            noteResult.ResultId = (int)id;
            return noteResult;
        }

        public void Update(Dataset dataset)
        {
            m_db.Update("dbo.Dataset", "dataset_id", dataset.GetTableObject(), dataset.DatasetId);
        }

        #region AmortizationSchedule.csv
        public string GetAmortizationSchedulePath(Project project, string asOfDate)
        {
            var datasetFolder = GetDatasetFolder(project, asOfDate);
            return datasetFolder + "AmortizationSchedule.csv";
        }

        public AmortizationSchedule LoadAmortizationSchedule(Project project, string asOfDate)
        {
            var filePath = GetAmortizationSchedulePath(project, asOfDate);
            AmortizationSchedule amortizationSchedule = new AmortizationSchedule();
            amortizationSchedule.Load(filePath);
            return amortizationSchedule;
        }

        public void SaveAmortizationSchedule(Project project, string asOfDate, AmortizationSchedule amortizationSchedule)
        {
            var filePath = GetAmortizationSchedulePath(project, asOfDate);
            amortizationSchedule.Save(filePath);
        }
        #endregion
    }
}
