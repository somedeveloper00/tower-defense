using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TowerDefense.Transport.Editor
{
    public class TransportablesConvert : EditorWindow
    {
        [MenuItem( "Window/TD/Transportables Convert" )]
        private static void ShowWindow() {
            var window = GetWindow<TransportablesConvert>();
            window.titleContent = new GUIContent( "Transportable Convert" );
            window.Show();
        }

        static Type[] _types;


        [InitializeOnLoadMethod]
        static void populateTypes() {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( a => a.GetTypes() ) 
                .Where( t => // check if has interface ITransportable
                    !t.IsAbstract && !t.IsSubclassOf( typeof(MonoBehaviour) ) &&
                    t.GetInterfaces().Contains( typeof(ITransportable) ) )
                .ToArray();
        }

        UnityEditor.Editor _editor;
        ScriptableObject _scriptableObject;
        TransportableConvertObjectHolder _transportableConvertObjectHolder;
        
        [SerializeField] string json;
        SerializedObject _thisSerializedObject;
        bool _prettyJson;

        [SerializeField] GUIStyle _selectionButtonStyle;
        [SerializeField] GUIStyle _jsonTextStyle;
        readonly Object[] _selectedObjects = new Object[8];

        private void OnGUI() {
            if ( _selectionButtonStyle is null || _selectionButtonStyle == null ||
                 _jsonTextStyle is null || _jsonTextStyle == null ) setupStyles();
            
            _thisSerializedObject ??= new SerializedObject( this );
            _thisSerializedObject.Update();

            drawTopBar();

            SplitterGUILayout.DrawHorizontal( "left-cent", 120, 
                leftPane: drawSelectionPane, 
                rightPane: () => {
                    SplitterGUILayout.DrawHorizontal( "cent-right", 340, drawObjectPanel, drawjsonPanel );
                } );

            _thisSerializedObject.ApplyModifiedProperties();
        }

        void setupStyles() {
            _selectionButtonStyle = new GUIStyle( GUI.skin.button );
            _selectionButtonStyle.alignment = TextAnchor.MiddleLeft;
            _selectionButtonStyle.fontStyle = FontStyle.Bold;
            _jsonTextStyle = new GUIStyle( GUI.skin.textArea );
            _jsonTextStyle.wordWrap = true;
        }

        void drawTopBar() {
            using (new GUILayout.HorizontalScope( GUILayout.Height( 25 ) )) {
                GUILayout.FlexibleSpace();
                if ( GUILayout.Button( "Copy json to clipboard" ) ) { EditorGUIUtility.systemCopyBuffer = json; }

                // toggle for pretty json
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    _prettyJson = GUILayout.Toggle( _prettyJson, "Pretty Json", EditorStyles.toolbarButton );
                    if ( check.changed ) objectToJson();
                }
            }
        }

        void drawSelectionPane() {
            var current = getCurrentObject();
            
            using (new GUILayout.VerticalScope()) {
                for (int i = 0; i < _types.Length; i++) {
                    using (new EditorGUI.DisabledScope( current != null && _types[i] == current.GetType() )) {

                        if ( GUILayout.Button( new GUIContent( _types[i].Name, "in " + _types[i].Namespace ),
                                _selectionButtonStyle, GUILayout.Height( 25 ) ) ) 
                        {
                            selectType( _types[i] );
                        }
                    }
                }
            }
        }

        void selectType(Type type) {
            if ( type.IsSubclassOf( typeof(ScriptableObject) ) ) {
                _scriptableObject = ScriptableObject.CreateInstance( type );
                _transportableConvertObjectHolder = null;
                _editor = UnityEditor.Editor.CreateEditor( _scriptableObject );
            }
            else {
                _transportableConvertObjectHolder = ScriptableObject.CreateInstance<TransportableConvertObjectHolder>();
                _transportableConvertObjectHolder.Object = (ITransportable)Activator.CreateInstance( type );
                _scriptableObject = null;
                _editor = UnityEditor.Editor.CreateEditor( _transportableConvertObjectHolder );
            }

            objectToJson();
            for (var k = 0; k < _selectedObjects.Length; k++) { _selectedObjects[k] = null; }
        }

        void drawObjectPanel() {
            GUILayout.Label( "Object", EditorStyles.largeLabel );
            if ( _editor != null ) {
                var current = getCurrentObject();
                var type = current.GetType();

                GUILayout.Label( type.FullName, EditorStyles.centeredGreyMiniLabel );
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    try {
                        _editor.OnInspectorGUI();
                        if ( check.changed ) objectToJson();
                    }
                    catch (Exception e) { Debug.LogException( e ); }
                }

                GUILayout.FlexibleSpace();

                // draw obejct copy field and button
                var exchangingTypes = type.GetInterfaces()
                    .Where( t => t.IsGenericType &&
                                 t.GetGenericTypeDefinition() == typeof(IDataExchange<>) &&
                                 t.GenericTypeArguments[0].IsSubclassOf( typeof(ScriptableObject) ) )
                    .ToList();
                if ( exchangingTypes.Count > 0 ) {
                    GUILayout.Label( "Data Exchange", EditorStyles.miniBoldLabel );

                    for (var i = 0; i < exchangingTypes.Count; i++) {
                        // caching types
                        var exchangingType = exchangingTypes[i];
                        var otherType = exchangingType.GenericTypeArguments[0];
                        // main box
                        using (new GUILayout.HorizontalScope( EditorStyles.helpBox, GUILayout.Height( 30 ) )) {
                            _selectedObjects[i] = EditorGUILayout.ObjectField( _selectedObjects[i], otherType, true,
                                GUILayout.ExpandHeight( true ) );

                            using (new EditorGUI.DisabledScope( _selectedObjects[i] == null )) {
                                using (new GUILayout.VerticalScope()) {
                                    // take from button
                                    if ( GUILayout.Button( "Take From" ) ) {
                                        var method = type.GetMethods()
                                            .Where( m => m.Name == nameof(IDataExchange<int>.TakeFrom) )
                                            .First( m => m.GetParameters()[0].ParameterType == otherType );
                                        method.Invoke( current, new object[] { _selectedObjects[i] } );
                                        _editor.serializedObject.Update();
                                        objectToJson();
                                    }

                                    // apply to button
                                    if ( GUILayout.Button( "Apply To" ) ) {
                                        var method = type.GetMethods()
                                            .Where( m => m.Name == nameof(IDataExchange<int>.ApplyTo) )
                                            .First( m => m.GetParameters()[0].ParameterType == otherType );
                                        method.Invoke( current, new object[] { _selectedObjects[i] } );
                                        _editor.serializedObject.Update();
                                        objectToJson();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else {
                if ( _types.Length > 0 )
                    selectType( _types[0] );
            }
        }

        ITransportable getCurrentObject() {
            if ( _transportableConvertObjectHolder != null && _transportableConvertObjectHolder.Object != null ) return _transportableConvertObjectHolder.Object;
            if ( _scriptableObject != null ) return _scriptableObject as ITransportable;
            selectType( _types[0] );
            return getCurrentObject();
        }

        void drawjsonPanel() {
            GUILayout.Label( "Json", EditorStyles.largeLabel );
            using (var check = new EditorGUI.ChangeCheckScope()) {
                var txtProp = _thisSerializedObject.FindProperty( nameof(json) );
                txtProp.stringValue =
                    EditorGUILayout.TextArea( txtProp.stringValue, _jsonTextStyle, GUILayout.ExpandHeight( true ) );
                if ( check.changed ) {
                    jsonToObject();
                    if ( _prettyJson ) prettifyJson();
                    _thisSerializedObject.Update();
                }
            }
        }

        void objectToJson() {
            if ( _editor != null ) _editor.serializedObject.ApplyModifiedProperties();
            ITransportable transportable = null;
            if ( _scriptableObject != null ) transportable = (ITransportable)_scriptableObject;
            else if ( _transportableConvertObjectHolder != null ) transportable = (ITransportable)_transportableConvertObjectHolder.Object;
            json = transportable == null ? string.Empty : transportable.ToJson();
            if ( _prettyJson ) prettifyJson();
            _thisSerializedObject.Update();
        }

        void prettifyJson() {
            var jobj = JsonConvert.DeserializeObject<JObject>( json );
            json = JsonConvert.SerializeObject( jobj, Formatting.Indented );
        }

        void jsonToObject() {
            _thisSerializedObject.ApplyModifiedProperties();
            ITransportable transportable = null;
            if ( _scriptableObject != null ) transportable = (ITransportable)_scriptableObject;
            else if ( _transportableConvertObjectHolder != null ) transportable = (ITransportable)_transportableConvertObjectHolder.Object;
            if ( transportable == null ) return;
            transportable.FromJson( json );
            _editor.serializedObject.Update();
        }
    }
}