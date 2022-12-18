using System;
using TaleWorlds.Library;

namespace TroopDesignerRewritten.ViewModel
{
    internal class TroopDesignerLoadFromCharacterId : TaleWorlds.Library.ViewModel
    {

        private string _characterId = "";
        private string _characterEquipmentSetId = "0";

        [DataSourceProperty]
        public string CharacterId
        {
            get => _characterId;
            set
            {

                this._characterId = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterId));
            }
        }

        [DataSourceProperty]
        public string CharacterEquipmentSetId
        {
            get => _characterEquipmentSetId;
            set
            {

                this._characterEquipmentSetId = value;
                this.OnPropertyChangedWithValue((object)value, nameof(CharacterEquipmentSetId));
            }
        }


        private Action<string, string> _onLoadCharacter;
        private bool _isOpen;

        public TroopDesignerLoadFromCharacterId(Action<string, string> onLoadCharacter)
        {
            _onLoadCharacter = onLoadCharacter;
            Close();
        }

        public void Open() => this.IsOpen = true;

        public void Close() => this.IsOpen = false;

        public void ExecuteCancel()
        {
            this.Close();
        }

        public void ExecuteClose()
        {
            _onLoadCharacter.Invoke(CharacterId, CharacterEquipmentSetId);
            this.Close();
        }

        [DataSourceProperty]
        public bool IsOpen
        {
            get => this._isOpen;
            set
            {
                if (value == this._isOpen)
                    return;
                this._isOpen = value;
                this.OnPropertyChangedWithValue((object)value, nameof(IsOpen));
            }
        }
    }
}
