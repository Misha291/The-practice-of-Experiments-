using System.Collections.Generic;

namespace StructBenchmarking
{
    public class Experiments
    {
        // Эксперимент: создание массивов
        public static ChartData BuildChartDataForArrayCreation(IBenchmark benchmark, int repetitionsCount)
        {
            return GenerateChartData(benchmark, repetitionsCount, new ArrayCreationTaskFactory());
        }

        // Эксперимент: передача в метод
        public static ChartData BuildChartDataForMethodCall(IBenchmark benchmark, int repetitionsCount)
        {
            return GenerateChartData(benchmark, repetitionsCount, new MethodCallTaskFactory());
        }

        // Общий код для обоих экспериментов — измеряем время и собираем результаты
        private static ChartData GenerateChartData(IBenchmark benchmark, int repetitionsCount, ITaskFactory factory)
        {
            var classMeasurements = new List<ExperimentResult>();
            var structMeasurements = new List<ExperimentResult>();

            foreach (var fieldCount in Constants.FieldCounts)
            {
                var classTask = factory.CreateClassTask(fieldCount);
                var structTask = factory.CreateStructTask(fieldCount);

                var classDuration = benchmark.MeasureDurationInMs(classTask, repetitionsCount);
                var structDuration = benchmark.MeasureDurationInMs(structTask, repetitionsCount);

                classMeasurements.Add(new ExperimentResult(fieldCount, classDuration));
                structMeasurements.Add(new ExperimentResult(fieldCount, structDuration));
            }

            return new ChartData
            {
                Title = factory.ChartTitle,
                ClassPoints = classMeasurements,
                StructPoints = structMeasurements,
            };
        }
    }

    // Этот интерфейс нужен, чтобы GenerateChartData не зависел от конкретных классов.
    // Он просто говорит: "Дай мне задачу для класса, задачу для структуры и название".
    public interface ITaskFactory
    {
        ITask CreateClassTask(int fieldCount);
        ITask CreateStructTask(int fieldCount);
        string ChartTitle { get; }
    }

    // Фабрика для эксперимента с созданием массивов.
    // Когда GenerateChartData просит задачу — она даёт нужную из ArrayCreationTasks.cs
    public class ArrayCreationTaskFactory : ITaskFactory
    {
        public ITask CreateClassTask(int fieldCount)
        {
            return new ClassArrayCreationTask(fieldCount);
        }

        public ITask CreateStructTask(int fieldCount)
        {
            return new StructArrayCreationTask(fieldCount);
        }

        public string ChartTitle
        {
            get { return "Array creation"; }
        }
    }

    // Фабрика для эксперимента с передачей аргумента в метод.
    // Аналогично — отдаёт задачи из MethodCallTasks.cs
    public class MethodCallTaskFactory : ITaskFactory
    {
        public ITask CreateClassTask(int fieldCount)
        {
            return new MethodCallWithClassArgumentTask(fieldCount);
        }

        public ITask CreateStructTask(int fieldCount)
        {
            return new MethodCallWithStructArgumentTask(fieldCount);
        }

        public string ChartTitle
        {
            get { return "Method call with argument"; }
        }
    }
}
