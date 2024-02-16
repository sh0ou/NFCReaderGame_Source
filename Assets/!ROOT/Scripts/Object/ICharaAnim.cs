using UnityEngine;

namespace Jubatus
{
    public interface ICharaAnim
    {
        protected Transform weaponHandBone_L { get; set; }
        protected Transform weaponHandBone_R { get; set; }

        protected Animation anim_move { get; set; }
        protected Animation anim_dodge { get; set; }
        protected Animation anim_jump { get; set; }
        protected Animation anim_attack { get; set; }
        protected Animation anim_damage { get; set; }
        protected Animation anim_dead { get; set; }
    }
}