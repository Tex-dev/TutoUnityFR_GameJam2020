using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet  m_planet;
    Editor  m_shapeEditor;
    Editor  m_colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if(check.changed)
            {
                m_planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
            m_planet.GeneratePlanet();


        DrawSettingsEditor(m_planet.m_shapeSettings, m_planet.OnShapeSettingsUpdated, ref m_planet.m_shapeSettingsFoldout, ref m_shapeEditor);
        DrawSettingsEditor(m_planet.m_colorSettings, m_planet.OnColorSettingsUpdated, ref m_planet.m_colorSettingsFoldout, ref m_colorEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        m_planet = (Planet)target;
    }
}
