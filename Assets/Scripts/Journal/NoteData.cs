using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                         NOTE DATA
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * ScriptableObject que define una nota o documento del juego.
 * Las notas se guardan en una pestana especial del Journal
 * separada de los items normales del inventario. 
 *
 * TIPOS DE NOTAS:
 *   - Paginas de diario
 *   - Notas de NPCs
 *   - Revistas y periodicos
 *   - Descripciones de inquilinos (osea, no se si lo querian pero pos nomas)
 *
 * SETUP:
 * Clic derecho en Assets -> Journal -> Note
 * NOTA: Pos no se si daran solo imagenes o tendremos que escribir pero pues para el futuro
 * ---------------------------------------------------------------
 */

[CreateAssetMenu(fileName = "NewNote", menuName = "Journal/Note")]
public class NoteData : ScriptableObject
{
    [Header("Informacion")]
    [Tooltip("Titulo de la nota")]
    public string noteTitle;

    [Tooltip("Tipo de nota")]
    public NoteType noteType;

    [Tooltip("Imagen de la nota (opcional)")]
    public Sprite noteImage;

    [Header("Contenido")]
    [Tooltip("Key de localizacion del contenido")]
    public string contentKey;

    [Tooltip("Key de localizacion del autor (opcional)")]
    public string authorKey;

    [Header("Settings")]
    [Tooltip("Si la nota tiene imagen grande al inspeccionarse")]
    public bool hasFullImage = false;
}

public enum NoteType
{
    DiaryPage,      // Pagina de diario
    NPCNote,        // Nota de NPC
    Magazine,       // Revista
    Newspaper,      // Periodico
    TenantProfile   // Perfil de inquilino
}