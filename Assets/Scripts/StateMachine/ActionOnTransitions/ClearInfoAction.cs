using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
  	[CreateAssetMenu(fileName = "ClearInfo Action", menuName = "configs/StateMachine/Actions/ClearInfo")]
  	public class ClearInfoAction : ActionOnTransitionBase
    {
         public override void DoAction(StateMachineContext context)
         {
             if (context.UsedInfoAboutPositionAtShelfForBuyer != null)
                 context.UsedInfoAboutPositionAtShelfForBuyer.IsFree = true;
             context.UsedInfoAboutPositionAtShelfForBuyer = null;
             // ReSharper disable once RedundantCheckBeforeAssignment
             if (context.TargetPoint != null)
                context.TargetPoint = null;
             // context.TP = context.TargetPoint.Target.position;
             // context.SetLongDistanceToTarget();
             //TODO: Кароче нужно использовать класс для получения точек спавна
             //Так же нужны переходы/состояния завязанные на изменение сетки/Объектов на сетке.
             //Так же нужен класс контролирующий входы в магазин. Что-то типо контроллера полок. То есть объекты сами себя
             //регистрируют в классе контроллере и сами выписывают.
             //А сейчас спать, а то времени уже - ебнешься.
         }
    }
}